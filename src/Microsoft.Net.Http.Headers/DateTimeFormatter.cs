// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Net.Http.Headers
{
    internal static class DateTimeFormatter
    {
        private static readonly DateTimeFormatInfo FormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;

        private static readonly string[] MonthNames = FormatInfo.AbbreviatedMonthNames;
        private static readonly string[] DayNames = FormatInfo.AbbreviatedDayNames;

        private static readonly int Rfc1123DateLength = "ddd, dd MMM yyyy HH:mm:ss GMT".Length;

        // ASCII numbers are in the range 48 - 57.
        private const int AsciiNumberOffset = 0x30;

        private const string Gmt = "GMT";
        private const char Comma = ',';
        private const char Space = ' ';
        private const char Colon = ':';

        public static unsafe string ToRfc1123String(this DateTimeOffset dateTime)
        {
            var offset = 0;
            var universal = dateTime.UtcDateTime;
            char* target = stackalloc char[Rfc1123DateLength];

            FormatDayOfWeek(universal.DayOfWeek, target, ref offset);
            Format(Comma, target, ref offset);
            Format(Space, target, ref offset);
            FormatNumber(universal.Day, target, ref offset);
            Format(Space, target, ref offset);
            FormatMonth(universal.Month, target, ref offset);
            Format(Space, target, ref offset);
            FormatYear(universal.Year, target, ref offset);
            Format(Space, target, ref offset);
            FormatTimeOfDay(universal.TimeOfDay, target, ref offset);
            Format(Space, target, ref offset);
            Format(Gmt, target, ref offset);

            return new string(target, 0, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatDayOfWeek(DayOfWeek dayOfWeek, char* target, ref int offset)
        {
            Format(DayNames[(int)dayOfWeek], target, ref offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatMonth(int month, char* target, ref int offset)
        {
            Format(MonthNames[month - 1], target, ref offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatYear(int year, char* target, ref int offset)
        {
            Format(GetAsciiChar(year / 1000), target, ref offset);
            Format(GetAsciiChar(year % 1000 / 100), target, ref offset);
            Format(GetAsciiChar(year % 100 / 10), target, ref offset);
            Format(GetAsciiChar(year % 10), target, ref offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatTimeOfDay(TimeSpan timeOfDay, char* target, ref int offset)
        {
            FormatNumber(timeOfDay.Hours, target, ref offset);
            Format(Colon, target, ref offset);
            FormatNumber(timeOfDay.Minutes, target, ref offset);
            Format(Colon, target, ref offset);
            FormatNumber(timeOfDay.Seconds, target, ref offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatNumber(int number, char* target, ref int offset)
        {
            Format(GetAsciiChar(number / 10), target, ref offset);
            Format(GetAsciiChar(number % 10), target, ref offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Format(string source, char* target, ref int offset)
        {
            for (var i = 0; i < source.Length; i++)
            {
                target[offset++] = source[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Format(char value, char* target, ref int offset)
        {
            target[offset++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char GetAsciiChar(int value)
        {
            return (char)(AsciiNumberOffset + value);
        }
    }
}
