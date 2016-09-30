// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Xunit;

namespace Microsoft.Net.Http.Headers
{
    public static class HeaderUtilitiesTest
    {
        private const string Rfc1123Format = "r";

        [Theory]
        [MemberData(nameof(TestValues))]
        public static void ReturnsSameResultAsRfc1123String(DateTimeOffset dateTime)
        {
            var expected = dateTime.ToString(Rfc1123Format);
            var actual = HeaderUtilities.FormatDate(dateTime);

            Assert.Equal(expected, actual);
        }

        public static TheoryData<DateTimeOffset> TestValues
        {
            get
            {
                var data = new TheoryData<DateTimeOffset>();

                var now = DateTimeOffset.Now;

                for (var i = 0; i < 60; i++)
                {
                    data.Add(now.AddSeconds(i));
                    data.Add(now.AddMinutes(i));
                    data.Add(now.AddDays(i));
                    data.Add(now.AddMonths(i));
                    data.Add(now.AddYears(i));
                }

                return data;
            }
        }
    }
}
