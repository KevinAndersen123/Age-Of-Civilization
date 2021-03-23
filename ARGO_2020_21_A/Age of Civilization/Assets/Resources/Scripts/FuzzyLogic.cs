using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class FuzzyLogic
{
    public static float FuzzyTriangle(float t_value, float t_x0, float t_x1, float t_x2)
    {
        return FuzzyTriangleLB(t_value, t_x0, t_x1, t_x2);
    }

    public static float FuzzyGradeUp(float t_value, float t_x0, float t_x1)
    {
        return FuzzyGradeUpLB(t_value, t_x0, t_x1);
    }

    public static float FuzzyGradeDown(float t_value, float t_x0, float t_x1)
    {
        return FuzzyGradeDownLB(t_value, t_x0, t_x1);
    }

    public static float FuzzyTrapezoid(float t_value, float t_x0, float t_x1, float t_x2, float t_x3)
    {
        return FuzzyTrapezoidLB(t_value, t_x0, t_x1, t_x2, t_x3);
    }

    public static float FuzzyAND(float t_value1, float t_value2)
    {
        return FuzzyANDLB(t_value1, t_value2);
    }

    public static float FuzzyOR(float t_value1, float t_value2)
    {
        return FuzzyORLB(t_value1, t_value2);
    }

    public static float FuzzyNOT(float t_value1)
    {
        return FuzzyNOTLB(t_value1);
    }

    [DllImport("FuzzyLogicLibrary", SetLastError = true)]
    static extern float FuzzyTriangleLB(float t_value, float t_x0, float t_x1, float t_x2);

    [DllImport("FuzzyLogicLibrary", SetLastError = true)]
    static extern float FuzzyGradeUpLB(float t_value, float t_x0, float t_x1);

    [DllImport("FuzzyLogicLibrary", SetLastError = true)]
    static extern float FuzzyGradeDownLB(float t_value, float t_x0, float t_x1);

    [DllImport("FuzzyLogicLibrary", SetLastError = true)]
    static extern float FuzzyTrapezoidLB(float t_value, float t_x0, float t_x1, float t_x2, float t_x3);

    [DllImport("FuzzyLogicLibrary", SetLastError = true)]
    static extern float FuzzyANDLB(float t_value1, float t_value2);

    [DllImport("FuzzyLogicLibrary", SetLastError = true)]
    static extern float FuzzyORLB(float t_value1, float t_value2);

    [DllImport("FuzzyLogicLibrary", SetLastError = true)]
    static extern float FuzzyNOTLB(float t_value1);
}
