using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class FuzzyLogicSuite
{
    [UnityTest, Order(0)]
    public IEnumerator FuzzyTriangleTest()
    {
        float x0 = 0.0f;
        float x1 = 5.0f;
        float x2 = 10.0f;

        float value = -1.0f;
        float returnedValue = FuzzyLogic.FuzzyTriangle(value, x0, x1, x2);

        Assert.True(returnedValue == 0.0f);

        value = 11.0f;
        returnedValue = FuzzyLogic.FuzzyTriangle(value, x0, x1, x2);

        Assert.True(returnedValue == 0.0f);

        value = 5.0f;
        returnedValue = FuzzyLogic.FuzzyTriangle(value, x0, x1, x2);

        Assert.True(returnedValue == 1.0f);

        value = 2.5f;
        returnedValue = FuzzyLogic.FuzzyTriangle(value, x0, x1, x2);

        Assert.True(returnedValue == 0.5f);

        yield return null;
    }

    [UnityTest, Order(1)]
    public IEnumerator FuzzyGradeUpTest()
    {
        float x0 = 0.0f;
        float x1 = 5.0f;

        float value = -1.0f;
        float returnedValue = FuzzyLogic.FuzzyGradeUp(value, x0, x1);

        Assert.True(returnedValue == 0.0f);

        value = 11.0f;
        returnedValue = FuzzyLogic.FuzzyGradeUp(value, x0, x1);

        Assert.True(returnedValue == 1.0f);

        value = 5.0f;
        returnedValue = FuzzyLogic.FuzzyGradeUp(value, x0, x1);

        Assert.True(returnedValue == 1.0f);

        value = 2.5f;
        returnedValue = FuzzyLogic.FuzzyGradeUp(value, x0, x1);

        Assert.True(returnedValue == 0.5f);

        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator FuzzyGradeDownTest()
    {
        float x0 = 0.0f;
        float x1 = 5.0f;

        float value = -1.0f;
        float returnedValue = FuzzyLogic.FuzzyGradeDown(value, x0, x1);

        Assert.True(returnedValue == 1.0f);

        value = 11.0f;
        returnedValue = FuzzyLogic.FuzzyGradeDown(value, x0, x1);

        Assert.True(returnedValue == 0.0f);

        value = 5.0f;
        returnedValue = FuzzyLogic.FuzzyGradeDown(value, x0, x1);

        Assert.True(returnedValue == 1.0f);

        value = 2.5f;
        returnedValue = FuzzyLogic.FuzzyGradeUp(value, x0, x1);

        Assert.True(returnedValue == 0.5f);

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator FuzzyTrapezoidTest()
    {
        float x0 = 0.0f;
        float x1 = 5.0f;
        float x2 = 10.0f;
        float x3 = 15.0f;

        float value = -1.0f;
        float returnedValue = FuzzyLogic.FuzzyTrapezoid(value, x0, x1, x2, x3);

        Assert.True(returnedValue == 0.0f);

        value = 16.0f;
        returnedValue = FuzzyLogic.FuzzyTrapezoid(value, x0, x1, x2, x3);

        Assert.True(returnedValue == 0.0f);

        value = 5.0f;
        returnedValue = FuzzyLogic.FuzzyTrapezoid(value, x0, x1, x2, x3);

        Assert.True(returnedValue == 1.0f);

        value = 8.0f;
        returnedValue = FuzzyLogic.FuzzyTrapezoid(value, x0, x1, x2, x3);

        Assert.True(returnedValue == 1.0f);

        value = 2.5f;
        returnedValue = FuzzyLogic.FuzzyTrapezoid(value, x0, x1, x2, x3);

        Assert.True(returnedValue == 0.5f);

        value = 12.5f;
        returnedValue = FuzzyLogic.FuzzyTrapezoid(value, x0, x1, x2, x3);

        Assert.True(returnedValue == 0.5f);

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator FuzzyORTest()
    {
        float value1 = 0.9f;
        float value2 = 0.1f;
        float returnedValue = FuzzyLogic.FuzzyOR(value1, value2);

        Assert.True(returnedValue == value1);

        value1 = 0.4f;
        value2 = 0.6f;
        returnedValue = FuzzyLogic.FuzzyOR(value1, value2);

        Assert.True(returnedValue == value2);

        value1 = 0.55f;
        value2 = 0.45f;
        returnedValue = FuzzyLogic.FuzzyOR(value1, value2);

        Assert.True(returnedValue == value1);

        yield return null;
    }


    [UnityTest, Order(5)]
    public IEnumerator FuzzyANDTest()
    {
        float value1 = 0.9f;
        float value2 = 0.1f;
        float returnedValue = FuzzyLogic.FuzzyAND(value1, value2);

        Assert.True(returnedValue == value2);

        value1 = 0.4f;
        value2 = 0.6f;
        returnedValue = FuzzyLogic.FuzzyAND(value1, value2);

        Assert.True(returnedValue == value1);

        value1 = 0.55f;
        value2 = 0.45f;
        returnedValue = FuzzyLogic.FuzzyAND(value1, value2);

        Assert.True(returnedValue == value2);

        yield return null;
    }


    [UnityTest, Order(6)]
    public IEnumerator FuzzyNOTTest()
    {
        float value = 0.4f;
        float returnedValue = FuzzyLogic.FuzzyNOT(value);

        Assert.True(returnedValue == 0.6f);

        value = 0.7f;
        returnedValue = FuzzyLogic.FuzzyNOT(value);

        Assert.True(returnedValue == 0.3f);

        value = 0.5f;
        returnedValue = FuzzyLogic.FuzzyNOT(value);

        Assert.True(returnedValue == 0.5f);

        value = 0.0f;
        returnedValue = FuzzyLogic.FuzzyNOT(value);

        Assert.True(returnedValue == 1.0f);

        value = 1.0f;
        returnedValue = FuzzyLogic.FuzzyNOT(value);

        Assert.True(returnedValue == 0.0f);

        yield return null;
    }

}
