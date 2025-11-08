using System;
using UnityEngine;

/// <summary>
/// 게임 내 모든 수식을 관리하는 클래스
/// Stage, Upgrade, Auto Collect, Rare Chance 등 통합
/// </summary>
public static class Formula
{
    public static int GetDifficultyLevel_FromMapManager()
    {
        int mapLevel  = MapManager.Instance.CurrentMapId;
        int stageLevel  = MapManager.Instance.CurrentStageId + 1;
        
        int difficultyLevel = mapLevel * 10 + stageLevel;
        
        return difficultyLevel;
    }

    public static double CalculateFishMaxHp(FishData fish, int difficultyLevel)
    {
        double defaultHp = fish.baseValue;
        double hpMultiplier = 1.12;   // 단계당 12% 증가
        double linearBonus = 2.0;     // 추가 선형 보정

        double maxHp = defaultHp * Math.Pow(hpMultiplier, difficultyLevel * linearBonus);
        
        return maxHp;
    }
    
    
    // -----------------------
    // 🔹 Stage 관련 수식
    // -----------------------

    /// <summary>
    /// 선형 증가 (Linear)
    /// 100 + stageId * 500
    /// </summary>
    public static int GetStageCatchLinear(int stageId)
    {
        return 100 + stageId * 500;
    }

    /// <summary>
    /// 곱연산 (Multiplicative)
    /// 100 * (stageId + 1)
    /// </summary>
    public static int GetStageCatchMultiplicative(int stageId)
    {
        return 100 * (stageId + 1);
    }

    /// <summary>
    /// 제곱 증가 (Quadratic)
    /// 100 + stageId^2 * 50
    /// </summary>
    public static int GetStageCatchQuadratic(int stageId)
    {
        return 100 + (stageId * stageId) * 50;
    }

    /// <summary>
    /// 지수 증가 (Exponential)
    /// 100 * 1.2^stageId
    /// </summary>
    public static int GetStageCatchExponential(int stageId)
    {
        return (int)(100 * Mathf.Pow(1.2f, stageId));
    }

    /// <summary>
    /// 로그 증가 (Logarithmic)
    /// 100 * log(stageId + 2)
    /// </summary>
    public static int GetStageCatchLogarithmic(int stageId)
    {
        return (int)(100 * Mathf.Log(stageId + 2));
    }

    /// <summary>
    /// 선형 + 제곱 혼합 (Linear + Quadratic)
    /// 100 + stageId * 200 + stageId^2 * 20
    /// </summary>
    public static int GetStageCatchLinearQuadratic(int stageId)
    {
        return 100 + stageId * 200 + (stageId * stageId) * 20;
    }

    // -----------------------
    // 🔹 Upgrade 관련 수식
    // -----------------------

    public static double GetClickValue(double baseValue, int level, bool additive = true)
    {
        return additive ? baseValue + level * 5 : baseValue * (1.0 + level * 0.05);
    }

    public static double GetAutoCollectMultiplier(int level)
    {
        return 1.0 + level * 0.2;
    }

    public static double GetRareDropChance(int level)
    {
        return Mathf.Min(5 + level * 2, 50);
    }

    public static long GetUpgradeCost(int level)
    {
        return (long)(100 * Mathf.Pow(1.5f, level));
    }

    // -----------------------
    // 🔹 기타 수식
    // -----------------------

    public static int GetPlayerLevel(int totalExp)
    {
        return totalExp / 1000;
    }

    public static long GetGoldReward(int stageId)
    {
        return (long)(50 * Mathf.Pow(1.2f, stageId));
    }
    
    public static long RoundToTwoMostSignificantDigits(long number)
    {
        if (number < 10) return number; // 1자리 수는 그대로

        // 자릿수 계산
        int digits = (int)Mathf.Floor(Mathf.Log10(number)) + 1;

        // 가장 큰 두 자리 숫자
        int divisor = (int)Mathf.Pow(10, digits - 2);

        // 나머지는 0으로
        long rounded = (number / divisor) * divisor;

        return rounded;
    }

}
