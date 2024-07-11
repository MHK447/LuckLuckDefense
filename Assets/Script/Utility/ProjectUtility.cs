using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using BanpoFri;
using UnityEngine.AddressableAssets;
using System.Globalization;
using System.IO;
public class ProjectUtility 
{

    private static string str_seconds;
    private static string str_minute;
    private static string str_hour;
    private static string str_day;

    public static void SetActiveCheck(GameObject target, bool value)
    {
        if (target != null)
        {
            if (value && !target.activeSelf)
                target.SetActive(true);
            else if (!value && target.activeSelf)
                target.SetActive(false);
        }
    }

    public static System.Numerics.BigInteger FibonacciDynamic(int n)
    {
        if (n <= 1)
            return n;

        System.Numerics.BigInteger[] fib = new System.Numerics.BigInteger[n + 1];
        fib[0] = 0;
        fib[1] = 1;

        for (int i = 2; i <= n; i++)
        {
            fib[i] = fib[i - 1] + fib[i - 2];
        }
        return fib[n];
    }

    private static char[] NumberChar =
    {
            'a','b','c','d','e','f','g','h','i','j','k',
            'l','m','n','o','p','q','r','s','t','u','v',
            'w','x','y','z'
        };

    public static string CalculateMoneyToString(System.Numerics.BigInteger _Long)
    {
        var targetString = _Long.ToString();
        var targetLen = targetString.Length - 1;
        if (targetLen == 0)
            targetLen = 1;
        var front = targetLen / 3;
        var back = targetLen % 3;
        if (front == 0)
        {
            return _Long.ToString();
        }
        var top = targetString.Substring(0, back + 1);
        var top_back = targetString.Substring(back + 1, 1);
        var top_back2 = targetString.Substring(back + 2, 1);

        var front_copy = front;
        if (front > 1378) // 26 + 26 * 26 + 26 * 26 + 26 * 26
        {
            front_copy = front_copy - 1378;
        }
        else if (front > 702) // 26 + 26 * 26
        {
            front_copy = front_copy - 702;
        }
        else if (front > 26)
        {
            front_copy = front_copy - 26;
        }

        var front_front = front_copy / 26;
        var front_second = front_copy % 26;

        char secondChar;
        if (front_second == 0)
        {
            secondChar = 'z';
            front_front = front_front - 1;
        }
        else if (front_second > 0 && front_second < 26)
            secondChar = NumberChar[front_second - 1];
        else
            secondChar = (char)0;

        char frontChar;
        if (front_front == 26)
            frontChar = 'z';
        else if (front_front >= 0 && front_front <= 26)
            frontChar = NumberChar[front_front];
        else
            frontChar = (char)0;

        string final_numTostr = string.Empty;

        if (front > 1378) // 26 + 26 * 26 + 26 * 26 + 26 * 26
            final_numTostr = $"{char.ToUpper(frontChar)}{char.ToUpper(secondChar)}";
        else if (front > 702) // 26 + 26 * 26 + 26 * 26 + 26 * 26
            final_numTostr = $"{char.ToUpper(frontChar)}{secondChar}";
        else if (front > 26)
            final_numTostr = $"{frontChar}{secondChar}";
        else
            final_numTostr = $"{secondChar}";

        if (top_back == "0" && top_back2 != "0")
            return string.Format("{0}.{1}{2}{3}", top, top_back, top_back2, final_numTostr);
        else if (top_back == "0" && top_back2 == "0")
            return string.Format("{0}{1}", top, final_numTostr);
        else if (top_back != "0" && top_back2 == "0")
            return string.Format("{0}.{1}{2}", top, top_back, final_numTostr);
        else
            return string.Format("{0}.{1}{2}{3}", top, top_back, top_back2, final_numTostr);

    }

    public static int GetRandGachaCard(int level)
    {

        int randgrade = 1;

        var td = Tables.Instance.GetTable<UnitGradeInfo>().GetData(level);

        int totalgacharatio = 0;

        if(td != null)
        {
            for(int i = 0; i < td.gradepercent.Count; ++i)
            {
                totalgacharatio += td.gradepercent[i];
            }


            var randgacha = UnityEngine.Random.Range(0, totalgacharatio + 1);
            int cumulativevalue = 0;

            for (int i = 0; i < td.gradepercent.Count; ++i)
            {
                cumulativevalue += td.gradepercent[i];

                if(randgacha < cumulativevalue)
                {
                    return i + 1;
                }

            }


        }

        return randgrade;
    }


    public static string GetTimeStringFormattingShort(int seconds)
    {
        str_seconds = Tables.Instance.GetTable<Localize>().GetString("str_time_second");
        str_minute = Tables.Instance.GetTable<Localize>().GetString("str_time_minute");
        str_hour = Tables.Instance.GetTable<Localize>().GetString("str_time_hour");
        str_day = Tables.Instance.GetTable<Localize>().GetString("str_time_day");

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var cnt = 0;
        var time = new TimeSpan(0, 0, seconds);
        if (time.Days > 0)
        {
            sb.Append(time.Days.ToString());
            sb.Append(str_day);
            ++cnt;
        }
        if (time.Hours > 0)
        {
            if (cnt > 0)
                sb.Append(" ");
            sb.Append(time.Hours.ToString());
            sb.Append(str_hour);

            ++cnt;
        }
        if (time.Minutes > 0 && cnt < 2)
        {
            if (cnt > 0)
                sb.Append(" ");
            sb.Append(time.Minutes.ToString());
            sb.Append(str_minute);
            ++cnt;
        }
        if (time.Seconds >= 0 && cnt < 2)
        {
            if (cnt > 0)
                sb.Append(" ");
            sb.Append(time.Seconds.ToString());
            sb.Append(str_seconds);
            ++cnt;
        }
        return sb.ToString();
    }

}
