namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class StatsProcsToOnlyConsiderCurrentAttemptsWhenTargetComplete : DbMigration
  {
    public override void Up()
    {
      DropStoredProcedure("dbo.sp_CalculateGoalAverageCompletionAcrossAllPeriods");
      DropStoredProcedure("dbo.sp_CalculateGoalAverageCompletionAcrossLast3Periods");

      Sql(
        "CREATE PROCEDURE dbo.sp_CalculateGoalAverageCompletionAcrossAllPeriods " +
          "@goalId int " +
        "AS " +
        "BEGIN " +
          "SET NOCOUNT ON; " +

          "DECLARE @startDate datetime2; " +
          "DECLARE @periodInHours int; " +
          "DECLARE @targetAttemptCount int; " +

          "SET @startDate = ( " +
            "SELECT StartDate " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "SET @periodInHours = ( " +
            "SELECT PeriodInHours " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "SET @targetAttemptCount = ( " +
            "SELECT FrequencyWithinPeriod " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "IF @startDate IS NULL RETURN 1000; " +
          "IF @periodInHours < 1 RETURN 1001; " +
          "IF @targetAttemptCount < 1 RETURN 1002; " +

          "DECLARE @now smalldatetime = GETUTCDATE(); " +
          "DECLARE @periodStart smalldatetime = @startDate; " +

          "CREATE TABLE #AttemptCounts " +
          "( " +
            "periodCount int " +
          "); " +

          "WHILE @periodStart < @now " +
          "BEGIN " +
            "DECLARE @periodEnd datetime = DATEADD(hh, @periodInHours, @periodStart); " +
            "SET @periodEnd = DATEADD(s, -1, @periodEnd); " +

            "DECLARE @attemptCount int = ( " +
              "SELECT COUNT(Id) " +
              "FROM dbo.GoalAttempts " +
              "WHERE " +
                "GoalId = @goalId AND " +
                "Timestamp BETWEEN @periodStart AND @periodEnd); " +

            "IF @periodEnd < @now OR @attemptCount >= @targetAttemptCount " +
            "BEGIN " +
              "INSERT INTO #AttemptCounts " +
              "VALUES(@attemptCount); " +
            "END " +

            "SET @periodStart = DATEADD(hh, @periodInHours, @periodStart); " +
          "END " +

          "DECLARE @avgAttemptsPerPeriod float = ( " +
            "SELECT AVG(CAST(periodCount AS float)) " +
            "FROM #AttemptCounts); " +

          "DROP TABLE #AttemptCounts; " +

          "IF @avgAttemptsPerPeriod IS NULL " +
          "BEGIN " +
            "SET @avgAttemptsPerPeriod = 0;" +
          "END " +

          "SELECT CAST((@avgAttemptsPerPeriod / @targetAttemptCount) * 100 AS int); " +
        "END");

      Sql(
        "CREATE PROCEDURE dbo.sp_CalculateGoalAverageCompletionAcrossLast3Periods " +
          "@goalId int " +
        "AS " +
        "BEGIN " +
          "SET NOCOUNT ON; " +

          "DECLARE @startDate datetime2; " +
          "DECLARE @periodInHours int; " +
          "DECLARE @targetAttemptCount int; " +

          "SET @startDate = ( " +
            "SELECT StartDate " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "SET @periodInHours = ( " +
            "SELECT PeriodInHours " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "SET @targetAttemptCount = ( " +
            "SELECT FrequencyWithinPeriod " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "IF @startDate IS NULL RETURN 1000; " +
          "IF @periodInHours < 1 RETURN 1001; " +
          "IF @targetAttemptCount < 1 RETURN 1002; " +

          "DECLARE @now smalldatetime = GETUTCDATE(); " +
          "DECLARE @periodStart smalldatetime = @startDate; " +

          "CREATE TABLE #AttemptCounts " +
          "( " +
            "id int IDENTITY(1,1), " +
            "periodCount int " +
          "); " +

          "WHILE @periodStart < @now " +
          "BEGIN " +
            "DECLARE @periodEnd datetime = DATEADD(hh, @periodInHours, @periodStart); " +
            "SET @periodEnd = DATEADD(s, -1, @periodEnd); " +

            "DECLARE @attemptCount int = ( " +
              "SELECT COUNT(Id) " +
              "FROM dbo.GoalAttempts " +
              "WHERE " +
                "GoalId = @goalId AND " +
                "Timestamp BETWEEN @periodStart AND @periodEnd); " +

            "IF @periodEnd < @now OR @attemptCount >= @targetAttemptCount " +
            "BEGIN " +
              "INSERT INTO #AttemptCounts " +
              "VALUES(@attemptCount); " +
            "END " +

            "SET @periodStart = DATEADD(hh, @periodInHours, @periodStart); " +
          "END " +

          "DECLARE @avgAttemptsForLast3Periods float = ( " +
            "SELECT AVG(CAST(LastPeriodCounts.periodCount AS float)) " +
            "FROM ( " +
              "SELECT TOP(3) periodCount " +
              "FROM #AttemptCounts " +
              "ORDER BY id DESC) LastPeriodCounts) " +

          "DROP TABLE #AttemptCounts; " +

          "IF @avgAttemptsForLast3Periods IS NULL " +
          "BEGIN " +
          "SET @avgAttemptsForLast3Periods = 0;" +
          "END " +

          "SELECT CAST((@avgAttemptsForLast3Periods / @targetAttemptCount) * 100 AS int); " +
        "END");
    }

    public override void Down()
    {
      DropStoredProcedure("dbo.sp_CalculateGoalAverageCompletionAcrossAllPeriods");
      DropStoredProcedure("dbo.sp_CalculateGoalAverageCompletionAcrossLast3Periods");

      Sql(
        "CREATE PROCEDURE dbo.sp_CalculateGoalAverageCompletionAcrossAllPeriods " +
          "@goalId int " +
        "AS " +
        "BEGIN " +
          "SET NOCOUNT ON; " +

          "DECLARE @startDate datetime2; " +
          "DECLARE @periodInHours int; " +
          "DECLARE @targetAttemptCount int; " +

          "SET @startDate = ( " +
            "SELECT StartDate " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "SET @periodInHours = ( " +
            "SELECT PeriodInHours " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "SET @targetAttemptCount = ( " +
            "SELECT FrequencyWithinPeriod " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "IF @startDate IS NULL RETURN 1000; " +
          "IF @periodInHours < 1 RETURN 1001; " +
          "IF @targetAttemptCount < 1 RETURN 1002; " +

          "DECLARE @now smalldatetime = GETUTCDATE(); " +
          "DECLARE @periodStart smalldatetime = @startDate; " +

          "CREATE TABLE #AttemptCounts " +
          "( " +
            "periodCount int " +
          "); " +

          "WHILE @periodStart < @now " +
          "BEGIN " +
            "DECLARE @periodEnd datetime = DATEADD(hh, @periodInHours, @periodStart); " +
            "SET @periodEnd = DATEADD(s, -1, @periodEnd); " +

            "DECLARE @attemptCount int = ( " +
              "SELECT COUNT(Id) " +
              "FROM dbo.GoalAttempts " +
              "WHERE " +
                "GoalId = @goalId AND " +
                "Timestamp BETWEEN @periodStart AND @periodEnd); " +

            "INSERT INTO #AttemptCounts " +
            "VALUES(@attemptCount); " +

            "SET @periodStart = DATEADD(hh, @periodInHours, @periodStart); " +
          "END " +

          "DECLARE @avgAttemptsPerPeriod float = ( " +
            "SELECT AVG(CAST(periodCount AS float)) " +
            "FROM #AttemptCounts); " +

          "DROP TABLE #AttemptCounts; " +

          "IF @avgAttemptsPerPeriod IS NULL " +
          "BEGIN " +
            "SET @avgAttemptsPerPeriod = 0;" +
          "END " +

          "SELECT CAST((@avgAttemptsPerPeriod / @targetAttemptCount) * 100 AS int); " +
        "END");

      Sql(
        "CREATE PROCEDURE dbo.sp_CalculateGoalAverageCompletionAcrossLast3Periods " +
          "@goalId int " +
        "AS " +
        "BEGIN " +
          "SET NOCOUNT ON; " +

          "DECLARE @startDate datetime2; " +
          "DECLARE @periodInHours int; " +
          "DECLARE @targetAttemptCount int; " +

          "SET @startDate = ( " +
            "SELECT StartDate " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "SET @periodInHours = ( " +
            "SELECT PeriodInHours " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "SET @targetAttemptCount = ( " +
            "SELECT FrequencyWithinPeriod " +
            "FROM dbo.Goals " +
            "WHERE Id = @goalId); " +

          "IF @startDate IS NULL RETURN 1000; " +
          "IF @periodInHours < 1 RETURN 1001; " +
          "IF @targetAttemptCount < 1 RETURN 1002; " +

          "DECLARE @now smalldatetime = GETUTCDATE(); " +
          "DECLARE @periodStart smalldatetime = @startDate; " +

          "CREATE TABLE #AttemptCounts " +
          "( " +
            "id int IDENTITY(1,1), " +
            "periodCount int " +
          "); " +

          "WHILE @periodStart < @now " +
          "BEGIN " +
            "DECLARE @periodEnd datetime = DATEADD(hh, @periodInHours, @periodStart); " +
            "SET @periodEnd = DATEADD(s, -1, @periodEnd); " +

            "DECLARE @attemptCount int = ( " +
              "SELECT COUNT(Id) " +
              "FROM dbo.GoalAttempts " +
              "WHERE " +
                "GoalId = @goalId AND " +
                "Timestamp BETWEEN @periodStart AND @periodEnd); " +

            "INSERT INTO #AttemptCounts " +
            "VALUES(@attemptCount); " +

            "SET @periodStart = DATEADD(hh, @periodInHours, @periodStart); " +
          "END " +

          "DECLARE @avgAttemptsForLast3Periods float = ( " +
            "SELECT AVG(CAST(LastPeriodCounts.periodCount AS float)) " +
            "FROM ( " +
              "SELECT TOP(3) periodCount " +
              "FROM #AttemptCounts " +
              "ORDER BY id DESC) LastPeriodCounts) " +

          "DROP TABLE #AttemptCounts; " +

          "IF @avgAttemptsForLast3Periods IS NULL " +
          "BEGIN " +
          "SET @avgAttemptsForLast3Periods = 0;" +
          "END " +

          "SELECT CAST((@avgAttemptsForLast3Periods / @targetAttemptCount) * 100 AS int); " +
        "END");
    }
  }
}
