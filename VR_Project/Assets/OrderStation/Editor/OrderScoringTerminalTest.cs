using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

// RUN THIS CODE THROUGH COMMAND PROMPT WITHOUT UNITY ON.
// THE PROMPT WILL BE LIKE THIS:
// "<their unity path>\Unity.exe" -batchmode -quit -projectPath "<their project path>" -executeMethod OrderScoringTerminalTest.Run -logFile "<where they want the log>"
//
// EXAMPLE:
// "C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe" -batchmode -quit -projectPath "C:\Users\arsal\VR-AR\VR_Project" -executeMethod OrderScoringTerminalTest.Run -logFile "C:\Users\arsal\VR-AR\VR_Project\Assets\OrderStation\order_test_log.txt"

public static class OrderScoringTerminalTest
{
    private static int failedChecks = 0;

    public static void Run()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

        Debug.Log("=== SCORING TEST START ===\n");

        RunDirectOrderScorerTests();
        RunOrderSystemSubmitAndTotalScoreTests();

        if (failedChecks == 0)
        {
            Debug.Log("\n=== SCORING TEST END: ALL CHECKS PASSED ===");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"\n=== SCORING TEST END: {failedChecks} CHECK(S) FAILED ===");
            EditorApplication.Exit(1);
        }
    }

    private static void RunDirectOrderScorerTests()
    {
        Debug.Log("\n=== PART 1: DIRECT ORDER SCORER TESTS ===");
        Debug.Log("These tests call OrderScorer directly and show how much each type of submission is worth.");
        Debug.Log("Imperfect submissions can still earn partial points if their final score stays above 0.\n");

        OrderGenerator generator = new OrderGenerator();
        float startTime = 0f;
        float completionTime = 10f;

        for (int mode = 1; mode <= 7; mode++)
        {
            Order testOrder = generator.GenerateOrder(mode, mode, startTime);

            Debug.Log($"MODE {mode} ORDER GENERATED (Limit: {testOrder.TimeLimit}s)");
            Debug.Log(FormatOrder(testOrder));

            List<ServedItem> perfectServed = BuildPerfectSubmission(testOrder);
            int perfectScore = OrderScorer.CalculateScore(testOrder, perfectServed, completionTime);
            Debug.Log($"SCENARIO 1 [Perfect Submission]: Score = {perfectScore}");
            Check(perfectScore > 0, $"Mode {mode} perfect submission should score above 0.");

            List<ServedItem> undercookedServed = BuildImperfectCookSubmission(testOrder, CookLevel.Undercooked);
            int undercookedScore = OrderScorer.CalculateScore(testOrder, undercookedServed, completionTime);
            Debug.Log($"SCENARIO 2 [Undercooked Submission / Partial Credit Possible]: Score = {undercookedScore}");

            List<ServedItem> missingIngredientServed = BuildMissingIngredientSubmission(testOrder);
            int missingIngredientScore = OrderScorer.CalculateScore(testOrder, missingIngredientServed, completionTime);
            Debug.Log($"SCENARIO 3 [Missing One Burger Ingredient / Partial Credit Possible]: Score = {missingIngredientScore}");

            List<ServedItem> overcookedServed = BuildImperfectCookSubmission(testOrder, CookLevel.Overcooked);
            int overcookedScore = OrderScorer.CalculateScore(testOrder, overcookedServed, completionTime);
            Debug.Log($"SCENARIO 4 [Overcooked Submission / May Still Earn Ingredient Points]: Score = {overcookedScore}");

            List<ServedItem> partialItems = BuildPartialSubmission(testOrder);
            int missingItemScore = OrderScorer.CalculateScore(testOrder, partialItems, completionTime);
            Debug.Log($"SCENARIO 5 [Missing One Entire Item / Partial Credit Possible]: Score = {missingItemScore}");

            List<ServedItem> extraItemSubmission = BuildExtraItemSubmission(testOrder);
            int extraItemScore = OrderScorer.CalculateScore(testOrder, extraItemSubmission, completionTime);
            Debug.Log($"SCENARIO 6 [Extra Item Penalty Applied]: Score = {extraItemScore}");

            List<ServedItem> emptySubmission = new List<ServedItem>();
            int emptyScore = OrderScorer.CalculateScore(testOrder, emptySubmission, completionTime);
            Debug.Log($"SCENARIO 7 [Empty Submission / Should Be 0]: Score = {emptyScore}");
            Check(emptyScore == 0, $"Mode {mode} empty submission should score 0.");

            Debug.Log("----------------------------------------\n");
        }
    }

    private static void RunOrderSystemSubmitAndTotalScoreTests()
    {
        Debug.Log("\n=== PART 2: ORDER SYSTEM SUBMIT + STORED SCORE + TOTAL SCORE TESTS ===");
        Debug.Log("These tests call OrderSystem.SubmitOrder, which is the same path used by the delivery button.");
        Debug.Log("If the calculated score is above 0, the order is completed and points are added to TotalScore.");
        Debug.Log("If the calculated score is 0, the order is failed and TotalScore does not change.");
        Debug.Log("If the order is expired, the current logic gives 0 because expired orders do not reach OrderScorer.\n");

        TestPerfectSubmissionStoresScoreAndAddsToTotal();
        TestMultipleSuccessfulOrdersAccumulateTotal();
        TestZeroScoreSubmissionDoesNotAddToTotal();
        TestExpiredSubmissionDoesNotReachScorerOrAddToTotal();
        TestMissingOrderSubmissionFailsSafely();
        TestTotalScoreChangedEventFiresOnSuccessfulSubmission();
    }

    private static void TestPerfectSubmissionStoresScoreAndAddsToTotal()
    {
        Debug.Log("\n--- TEST 1: Perfect submission stores score and adds to total ---");

        OrderSystem orderSystem = new OrderSystem();

        float startTime = 0f;
        float completionTime = 10f;

        Order order = orderSystem.CreateOrder(3, startTime);
        Debug.Log("Created order:");
        Debug.Log(FormatOrder(order));

        List<ServedItem> servedItems = BuildPerfectSubmission(order);
        int expectedScore = OrderScorer.CalculateScore(order, servedItems, completionTime);

        bool success = orderSystem.SubmitOrder(order.OrderId, servedItems, completionTime);

        Debug.Log($"Expected score: {expectedScore}");
        Debug.Log($"Actual LastSubmittedScore: {orderSystem.LastSubmittedScore}");
        Debug.Log($"Actual TotalScore: {orderSystem.TotalScore}");
        Debug.Log($"Submit success: {success}");
        Debug.Log($"Remaining active orders: {orderSystem.GetActiveOrders().Count}");

        Check(success, "Perfect submission should return true.");
        Check(orderSystem.LastSubmittedScore == expectedScore, "LastSubmittedScore should equal calculated score.");
        Check(orderSystem.TotalScore == expectedScore, "TotalScore should equal calculated score after one successful order.");
        Check(orderSystem.GetActiveOrders().Count == 0, "Submitted order should be removed from active orders.");
    }

    private static void TestMultipleSuccessfulOrdersAccumulateTotal()
    {
        Debug.Log("\n--- TEST 2: Multiple successful orders accumulate total score ---");

        OrderSystem orderSystem = new OrderSystem();

        float startTime = 0f;
        float completionTime = 10f;

        Order firstOrder = orderSystem.CreateOrder(3, startTime);
        List<ServedItem> firstServedItems = BuildPerfectSubmission(firstOrder);
        int firstExpectedScore = OrderScorer.CalculateScore(firstOrder, firstServedItems, completionTime);

        bool firstSuccess = orderSystem.SubmitOrder(firstOrder.OrderId, firstServedItems, completionTime);

        Order secondOrder = orderSystem.CreateOrder(4, startTime);
        List<ServedItem> secondServedItems = BuildPerfectSubmission(secondOrder);
        int secondExpectedScore = OrderScorer.CalculateScore(secondOrder, secondServedItems, completionTime);

        bool secondSuccess = orderSystem.SubmitOrder(secondOrder.OrderId, secondServedItems, completionTime);

        int expectedTotal = firstExpectedScore + secondExpectedScore;

        Debug.Log($"First expected score: {firstExpectedScore}");
        Debug.Log($"Second expected score: {secondExpectedScore}");
        Debug.Log($"Expected total: {expectedTotal}");
        Debug.Log($"Actual total: {orderSystem.TotalScore}");
        Debug.Log($"First success: {firstSuccess}");
        Debug.Log($"Second success: {secondSuccess}");
        Debug.Log($"LastSubmittedScore after second order: {orderSystem.LastSubmittedScore}");

        Check(firstSuccess, "First perfect order should succeed.");
        Check(secondSuccess, "Second perfect order should succeed.");
        Check(orderSystem.LastSubmittedScore == secondExpectedScore, "LastSubmittedScore should equal second order score after second submission.");
        Check(orderSystem.TotalScore == expectedTotal, "TotalScore should equal the sum of both successful orders.");
    }

    private static void TestZeroScoreSubmissionDoesNotAddToTotal()
    {
        Debug.Log("\n--- TEST 3: Zero-score submission fails and does not add to total score ---");

        OrderSystem orderSystem = new OrderSystem();

        float startTime = 0f;
        float completionTime = 10f;

        Order goodOrder = orderSystem.CreateOrder(3, startTime);
        List<ServedItem> goodServedItems = BuildPerfectSubmission(goodOrder);
        int expectedGoodScore = OrderScorer.CalculateScore(goodOrder, goodServedItems, completionTime);

        bool goodSuccess = orderSystem.SubmitOrder(goodOrder.OrderId, goodServedItems, completionTime);

        int totalBeforeZeroScoreOrder = orderSystem.TotalScore;

        Order zeroScoreOrder = orderSystem.CreateOrder(3, startTime);
        List<ServedItem> emptySubmission = new List<ServedItem>();

        bool zeroScoreSuccess = orderSystem.SubmitOrder(zeroScoreOrder.OrderId, emptySubmission, completionTime);

        Debug.Log($"Good order expected score: {expectedGoodScore}");
        Debug.Log($"Good order success: {goodSuccess}");
        Debug.Log($"Total before zero-score order: {totalBeforeZeroScoreOrder}");
        Debug.Log($"Zero-score order LastSubmittedScore: {orderSystem.LastSubmittedScore}");
        Debug.Log($"Total after zero-score order: {orderSystem.TotalScore}");
        Debug.Log($"Zero-score submit success value: {zeroScoreSuccess}");

        Check(goodSuccess, "Good order should succeed.");
        Check(!zeroScoreSuccess, "Empty submission should return false because its calculated score is 0.");
        Check(orderSystem.LastSubmittedScore == 0, "LastSubmittedScore should be 0 after a zero-score submission.");
        Check(orderSystem.TotalScore == totalBeforeZeroScoreOrder, "TotalScore should not change after a zero-score submission.");
    }

    private static void TestExpiredSubmissionDoesNotReachScorerOrAddToTotal()
    {
        Debug.Log("\n--- TEST 4: Expired submission gives 0 and does not add to total ---");

        OrderSystem orderSystem = new OrderSystem();

        float startTime = 0f;

        Order order = orderSystem.CreateOrder(3, startTime);
        List<ServedItem> servedItems = BuildPerfectSubmission(order);

        float expiredCompletionTime = order.TimeLimit + 5f;

        bool success = orderSystem.SubmitOrder(order.OrderId, servedItems, expiredCompletionTime);

        Debug.Log($"Order time limit: {order.TimeLimit}");
        Debug.Log($"Expired completion time: {expiredCompletionTime}");
        Debug.Log("Expected behavior: expired orders are marked expired before scoring, so no partial points are awarded.");
        Debug.Log($"LastSubmittedScore: {orderSystem.LastSubmittedScore}");
        Debug.Log($"TotalScore: {orderSystem.TotalScore}");
        Debug.Log($"Submit success: {success}");
        Debug.Log($"Remaining active orders: {orderSystem.GetActiveOrders().Count}");

        Check(!success, "Expired submission should return false.");
        Check(orderSystem.LastSubmittedScore == 0, "LastSubmittedScore should be 0 after expired submission.");
        Check(orderSystem.TotalScore == 0, "TotalScore should stay 0 after expired submission.");
        Check(orderSystem.GetActiveOrders().Count == 0, "Expired submitted order should be removed from active orders.");
    }

    private static void TestMissingOrderSubmissionFailsSafely()
    {
        Debug.Log("\n--- TEST 5: Missing order ID fails safely ---");

        OrderSystem orderSystem = new OrderSystem();

        List<ServedItem> servedItems = new List<ServedItem>
        {
            new ServedItem(FoodType.Fries, CookLevel.Cooked)
        };

        bool success = orderSystem.SubmitOrder(999, servedItems, 10f);

        Debug.Log($"Submit success: {success}");
        Debug.Log($"LastSubmittedScore: {orderSystem.LastSubmittedScore}");
        Debug.Log($"TotalScore: {orderSystem.TotalScore}");

        Check(!success, "Submitting a missing order ID should return false.");
        Check(orderSystem.LastSubmittedScore == 0, "LastSubmittedScore should be 0 after missing order submission.");
        Check(orderSystem.TotalScore == 0, "TotalScore should stay 0 after missing order submission.");
    }

    private static void TestTotalScoreChangedEventFiresOnSuccessfulSubmission()
    {
        Debug.Log("\n--- TEST 6: OnTotalScoreChanged fires only when total score increases ---");

        OrderSystem orderSystem = new OrderSystem();

        int eventFireCount = 0;
        int lastEventTotal = -1;

        orderSystem.OnTotalScoreChanged += total =>
        {
            eventFireCount++;
            lastEventTotal = total;
            Debug.Log($"[EVENT] OnTotalScoreChanged fired. New total: {total}");
        };

        Order order = orderSystem.CreateOrder(3, 0f);
        List<ServedItem> servedItems = BuildPerfectSubmission(order);
        int expectedScore = OrderScorer.CalculateScore(order, servedItems, 10f);

        bool success = orderSystem.SubmitOrder(order.OrderId, servedItems, 10f);

        Debug.Log($"Submit success: {success}");
        Debug.Log($"Expected score: {expectedScore}");
        Debug.Log($"Event fire count: {eventFireCount}");
        Debug.Log($"Last event total: {lastEventTotal}");

        Check(success, "Successful submission should return true.");
        Check(eventFireCount == 1, "OnTotalScoreChanged should fire exactly once for one successful submission.");
        Check(lastEventTotal == expectedScore, "OnTotalScoreChanged should pass the new total score.");
    }

    private static List<ServedItem> BuildPerfectSubmission(Order order)
    {
        List<ServedItem> served = new List<ServedItem>();

        foreach (OrderItemRequest req in order.RequestedItems)
        {
            served.Add(BuildServedItemFromRequest(req, req.RequiredCookLevel, req.Ingredients));
        }

        return served;
    }

    private static List<ServedItem> BuildImperfectCookSubmission(Order order, CookLevel level)
    {
        List<ServedItem> served = new List<ServedItem>();

        foreach (OrderItemRequest req in order.RequestedItems)
        {
            served.Add(BuildServedItemFromRequest(req, level, req.Ingredients));
        }

        return served;
    }

    private static List<ServedItem> BuildMissingIngredientSubmission(Order order)
    {
        List<ServedItem> served = new List<ServedItem>();
        bool removedIngredientAlready = false;

        foreach (OrderItemRequest req in order.RequestedItems)
        {
            if (req.FoodType == FoodType.Burger && req.Ingredients.Count > 0 && !removedIngredientAlready)
            {
                List<BurgerIngredients> modifiedIngredients = new List<BurgerIngredients>(req.Ingredients);
                modifiedIngredients.RemoveAt(0);
                removedIngredientAlready = true;

                served.Add(BuildServedItemFromRequest(req, req.RequiredCookLevel, modifiedIngredients));
            }
            else
            {
                served.Add(BuildServedItemFromRequest(req, req.RequiredCookLevel, req.Ingredients));
            }
        }

        return served;
    }

    private static List<ServedItem> BuildPartialSubmission(Order order)
    {
        List<ServedItem> served = BuildPerfectSubmission(order);

        if (served.Count > 0)
        {
            served.RemoveAt(served.Count - 1);
        }

        return served;
    }

    private static List<ServedItem> BuildExtraItemSubmission(Order order)
    {
        List<ServedItem> served = BuildPerfectSubmission(order);

        served.Add(new ServedItem(
            FoodType.Fries,
            CookLevel.Cooked
        ));

        return served;
    }

    private static ServedItem BuildServedItemFromRequest(
        OrderItemRequest req,
        CookLevel cookLevel,
        List<BurgerIngredients> ingredients
    )
    {
        if (req.FoodType == FoodType.Burger)
        {
            return new ServedItem(
                req.FoodType,
                cookLevel,
                req.PattyCount,
                ingredients
            );
        }

        return new ServedItem(
            req.FoodType,
            cookLevel
        );
    }

    private static string FormatOrder(Order order)
    {
        StringBuilder sb = new StringBuilder();

        foreach (OrderItemRequest item in order.RequestedItems)
        {
            if (item.FoodType == FoodType.Burger)
            {
                string ingredients = item.Ingredients.Count > 0
                    ? string.Join(" > ", item.Ingredients)
                    : "None";

                sb.AppendLine($"- Burger ({item.PattyCount}P) layers: {ingredients}");
            }
            else
            {
                sb.AppendLine($"- {item.FoodType}");
            }
        }

        return sb.ToString();
    }

    private static void Check(bool condition, string message)
    {
        if (condition)
        {
            Debug.Log($"[PASS] {message}");
        }
        else
        {
            failedChecks++;
            Debug.LogError($"[FAIL] {message}");
        }
    }
}