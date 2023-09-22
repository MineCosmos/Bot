﻿using System;
using System.Collections.Generic;
using Kook;
using Kook.Commands;
using Kook.Rest;
using Kook.WebSocket;
using MineCosmos.Bot.Common;
using Newtonsoft.Json;

namespace MineCosmos.Bot.Common;
/// <summary>
/// 
/// </summary>
public static class UtilConvert
{
    #region Kook扩展

    /// <summary>
    /// 统一底部
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static CardBuilder CardBuilderfoot(this CardBuilder builder)
    {
        return builder.AddModule<SectionModuleBuilder>(a =>
                    a.Text = new KMarkdownElementBuilder
                    {                        
                        Content = "(font)MineCosMos(font)[success] (font)私有宇宙(font)[purple] (font)无限可能(font)[warning] (font)脑洞大开(font)[pink]"
                    });
    }

    public static ICard KookSuccessCard(this string str)
    {
        string jsonData = $@"[
          {{
            ""type"": ""card"",
            ""theme"": ""success"",
            ""size"": ""lg"",
            ""modules"": [
              {{
                ""type"": ""section"",
                ""text"": {{
                  ""type"": ""kmarkdown"",
                  ""content"": ""{str}""
                }}
              }}     
            ]
          }}
        ]";
        return CardJsonExtension.ParseMany(jsonData).FirstOrDefault()!.Build();

        //.Build();
    }

    public static async Task<ICard> ReplaceMcTextToKook(this Task<string> str)
    {
        var result = await str?? "❤";
        if (str.IsNullOrEmpty()) result = "执行成功";

        if (result.Contains("§c"))
        {
            result = result.Replace("§c", string.Empty);
            result = result.Colorize(TextTheme.Danger);
        }

        if (result.Contains("[X]"))
        {
            result = result.Replace("[X]", "✅");
        }

        result = result.Replace("\\", "\\\\");
        return KookSuccessCard(result);
    }



    public static async Task ExecuteKookError(this SocketMessage message, string errorMsg = "", string emjoi = "❌")
    {
        await message.AddReactionAsync(new Kook.Emoji(emjoi));
        if (errorMsg.IsNotEmptyOrNull())
            await message.Channel.SendTextAsync(errorMsg);
    }

    // 🥵 👌

    public static async Task ExecuteKookSuccess(this SocketMessage message, string successMsg = "", string emjoi = "✅")
    {
        await message.AddReactionAsync(new Kook.Emoji(emjoi));
        if (successMsg.IsNotEmptyOrNull())
            await message.Channel.SendCardAsync(successMsg.KookSuccessCard());
    }

    public static async Task ExecuteKookSuccess(this SocketMessage message, ICard successMsg, string emjoi = "✅")
    {
        await message.AddReactionAsync(new Kook.Emoji(emjoi));
        if (successMsg.IsNotEmptyOrNull())
            await message.Channel.SendCardAsync(successMsg);
    }
    #endregion


    public static void PrintSuccess(this string str) => ConsoleHelper.SuccessMessage(str);
    public static void PrintWarning(this string str) => ConsoleHelper.WarningMessage(str);
    public static void PrintError(this string str) => ConsoleHelper.ErrorMessage(str);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static int ObjToInt(this object thisValue)
    {
        int reval = 0;
        if (thisValue == null) return 0;
        if (thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return reval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static int ObjToInt(this object thisValue, int errorValue)
    {
        int reval = 0;
        if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return errorValue;
    }

    public static long ObjToLong(this object thisValue)
    {
        long reval = 0;
        if (thisValue == null) return 0;
        if (thisValue != DBNull.Value && long.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return reval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static double ObjToMoney(this object thisValue)
    {
        double reval = 0;
        if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static double ObjToMoney(this object thisValue, double errorValue)
    {
        double reval = 0;
        if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return errorValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static string ObjToString(this object thisValue)
    {
        if (thisValue != null) return thisValue.ToString().Trim();
        return "";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static bool IsNotEmptyOrNull(this object thisValue)
    {
        return ObjToString(thisValue) != "" && ObjToString(thisValue) != "undefined" && ObjToString(thisValue) != "null";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static string ObjToString(this object thisValue, string errorValue)
    {
        if (thisValue != null) return thisValue.ToString().Trim();
        return errorValue;
    }

    public static bool IsNullOrEmpty(this object thisValue) => thisValue == null || thisValue == DBNull.Value || string.IsNullOrWhiteSpace(thisValue.ToString());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static Decimal ObjToDecimal(this object thisValue)
    {
        Decimal reval = 0;
        if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static Decimal ObjToDecimal(this object thisValue, decimal errorValue)
    {
        Decimal reval = 0;
        if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return errorValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static DateTime ObjToDate(this object thisValue)
    {
        DateTime reval = DateTime.MinValue;
        if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
        {
            reval = Convert.ToDateTime(thisValue);
        }

        return reval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static DateTime ObjToDate(this object thisValue, DateTime errorValue)
    {
        DateTime reval = DateTime.MinValue;
        if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return errorValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static bool ObjToBool(this object thisValue)
    {
        bool reval = false;
        if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return reval;
    }


    /// <summary>
    /// 获取当前时间的时间戳
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static string DateToTimeStamp(this DateTime thisValue)
    {
        TimeSpan ts = thisValue - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    public static object ChangeType(this object value, Type type)
    {
        if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
        if (value == null) return null;
        if (type == value.GetType()) return value;
        if (type.IsEnum)
        {
            if (value is string)
                return Enum.Parse(type, value as string);
            else
                return Enum.ToObject(type, value);
        }

        if (!type.IsInterface && type.IsGenericType)
        {
            Type innerType = type.GetGenericArguments()[0];
            object innerValue = ChangeType(value, innerType);
            return Activator.CreateInstance(type, new object[] { innerValue });
        }

        if (value is string && type == typeof(Guid)) return new Guid(value as string);
        if (value is string && type == typeof(Version)) return new Version(value as string);
        if (!(value is IConvertible)) return value;
        return Convert.ChangeType(value, type);
    }

    public static object ChangeTypeList(this object value, Type type)
    {
        if (value == null) return default;

        var gt = typeof(List<>).MakeGenericType(type);
        dynamic lis = Activator.CreateInstance(gt);

        var addMethod = gt.GetMethod("Add");
        string values = value.ToString();
        if (values != null && values.StartsWith("(") && values.EndsWith(")"))
        {
            string[] splits;
            if (values.Contains("\",\""))
            {
                splits = values.Remove(values.Length - 2, 2)
                    .Remove(0, 2)
                    .Split("\",\"");
            }
            else
            {
                splits = values.Remove(0, 1)
                    .Remove(values.Length - 2, 1)
                    .Split(",");
            }

            foreach (var split in splits)
            {
                var str = split;
                if (split.StartsWith("\"") && split.EndsWith("\""))
                {
                    str = split.Remove(0, 1)
                        .Remove(split.Length - 2, 1);
                }

                addMethod.Invoke(lis, new object[] { ChangeType(str, type) });
            }
        }

        return lis;
    }

    public static string ToJson(this object value)
    {
        return JsonConvert.SerializeObject(value);
    }
}
