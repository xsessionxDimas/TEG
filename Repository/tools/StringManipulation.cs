using System;
using System.Globalization;
using System.Security.Cryptography;

namespace Repository.tools
{
    public static class StringManipulation
    {
        public static string[] RomeNumberArray = new[]
                                                     {
                                                         "", 
                                                         "I", 
                                                         "II", 
                                                         "III", 
                                                         "IV", 
                                                         "V", 
                                                         "VI", 
                                                         "VII", 
                                                         "VIII", 
                                                         "IX", 
                                                         "X",
                                                         "XI", 
                                                         "XII"
                                                     };

        private static readonly string[] angka = new[]
                                                     {
                                                         "",
                                                         "Satu",
                                                         "Dua",
                                                         "Tiga",
                                                         "Empat",
                                                         "Lima",
                                                         "Enam",
                                                         "Tujuh",
                                                         "Delapan",
                                                         "Sembilan",
                                                         "Sepuluh"
                                                     };

        private static readonly string[] belasan = new[]
                                                      {
                                                          "",
                                                          "Sebelas",
                                                          "Dua Belas",
                                                          "Tiga Belas",
                                                          "Empat Belas",
                                                          "Lima Belas",
                                                          "Enam Belas",
                                                          "Tujuh Belas",
                                                          "Delapan Belas",
                                                          "Sembilan Belas"
                                                      };

        private static readonly string[] puluhan = new[]
                                                       {
                                                           "",
                                                           "Dua Puluh",
                                                           "Tiga Puluh",
                                                           "Empat Puluh",
                                                           "Lima Puluh",
                                                           "Enam Puluh",
                                                           "Tujuh Puluh",
                                                           "Delapan Puluh",
                                                           "Sembilan Puluh"
                                                       };

        private static readonly string[] kaya = new[]
                                                    {
                                                        "Ratus",
                                                        "Ribu",
                                                        "Juta",
                                                        "Milyar",
                                                        "Trilyun"
                                                    };

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            s = s.ToLower();
            char[] array = s.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] != ' ' || array[i - 1]  != '.') continue;
                if (char.IsLower(array[i]))
                {
                    array[i] = char.ToUpper(array[i]);
                }
            }
            return new string(array);
        }

        public static string ChangeToRomeNumber(int arrayIndex)
        {
            return RomeNumberArray[arrayIndex];
        }

        public static string NumToWords(long iNumericValues)
        {
            int count       = iNumericValues.ToString(CultureInfo.InvariantCulture).Length;
            string temp     = iNumericValues.ToString(CultureInfo.InvariantCulture);
            string szResult = "";
            for (var i = 0; i < count; i++)
            {
                var currentSum = Convert.ToInt64(temp.Substring(i));
                var currentNum = Convert.ToInt64(temp.Substring(i, 1));
                if (currentSum < 11)
                {
                    if (currentNum == 0) continue;
                    szResult += angka[currentSum] + " ";
                }
                else if (currentSum < 20)
                {
                    if (currentNum == 0) continue;
                    var x = Convert.ToInt64(currentSum.ToString(CultureInfo.InvariantCulture).Substring(1, 1));
                    szResult += belasan[x] + " ";
                    break;
                }
                else if (currentSum < 100)
                {
                    if (currentNum == 0) continue;
                    szResult += puluhan[currentNum - 1] + " ";
                }
                else if (currentSum < 1000)
                {
                    if (currentNum == 0) continue;
                    szResult += angka[currentNum] + " " + kaya[0] + " ";
                }

                else if (currentSum < 10000)
                {
                    if (currentNum == 0) continue;
                    szResult += angka[currentNum] + " " + kaya[1] + " ";
                    if (szResult.Contains("Satu Ribu"))
                    {
                        szResult = szResult.Replace("Satu Ribu", "Seribu");
                    }

                }
                else if (currentSum < 100000)
                {
                    if (currentNum == 0) continue;
                    var x = currentSum.ToString(CultureInfo.InvariantCulture).Substring(0, 2);
                    szResult += NumToWords(Convert.ToInt64(x)).TrimEnd() + " " + kaya[1] + " ";
                    i += 1;
                }

                else if (currentSum < 1000000)
                {
                    if (currentNum == 0) continue;
                    var x = currentSum.ToString(CultureInfo.InvariantCulture).Substring(1, 1);
                    if (x == "0")
                    {
                        szResult += angka[currentNum] + " " + kaya[0] + " " + kaya[1] + " ";
                        i += 1;
                    }
                    else
                    {
                        szResult += angka[currentNum] + " " + kaya[0] + " ";
                    }

                }

                else if (currentSum < 10000000)
                {
                    if (currentNum == 0) continue;
                    szResult += angka[currentNum] + " " + kaya[2] + " ";
                }

                else if (currentSum < 100000000)
                {
                    if (currentNum == 0) continue;
                    var x = currentSum.ToString(CultureInfo.InvariantCulture).Substring(0, 2);
                    szResult += NumToWords(Convert.ToInt64(x)).TrimEnd() + " " + kaya[2] + " ";
                    i += 1;
                }

                else if (currentSum < 1000000000)
                {
                    if (currentNum == 0) continue;
                    var x = currentSum.ToString(CultureInfo.InvariantCulture).Substring(1, 1);
                    var y = currentSum.ToString(CultureInfo.InvariantCulture).Substring(2, 1);
                    if (x == "0")
                    {
                        szResult += angka[currentNum] + " " + kaya[0] + " ";
                        i += 1;
                        if (y == "0")
                        {
                            szResult += kaya[2] + " ";
                            i += 1;
                        }
                    }
                    else
                    {
                        szResult += angka[currentNum] + " " + kaya[0] + " ";
                    }
                }

                else if (currentSum < 10000000000)
                {
                    if (currentNum == 0) continue;
                    szResult += angka[currentNum] + " " + kaya[3] + " ";
                }
            }
            if (szResult.Contains("Satu Ratus"))
            {
                szResult = szResult.Replace("Satu Ratus", "Seratus");
            }
            return szResult;
        } 
    }

    public static class FilenameGeneratorUtility
    {
        public static string SalesByMarketingExcelFilename()
        {
            return "Sales_By_Marketing" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string SalesByOutletExcelFilename()
        {
            return "Sales_By_Outlet" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string SalesByItemExcelFilename()
        {
            return "Sales_By_Item" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string ProductPricelistExcelFilename()
        {
            return "Pricelist" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string ProductStockExcelFilename()
        {
            return "Product_Stock" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string CustomerControlExcelFilename()
        {
            return "Customer_Control" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string CashFlowExcelFilename()
        {
            return "Cash_Flow" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string BankFlowExcelFilename()
        {
            return "Bank_Flow" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string StockFlowExcelFilename()
        {
            return "Stock_Flow" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string MoneyFlowExcelFilename()
        {
            return "Money_Flow" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string CashAdjustmentExcelFilename()
        {
            return "Cash_Adjustments" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string BankAdjustmentExcelFilename()
        {
            return "Bank_Adjustments" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string StockAdjustmentExcelFilename()
        {
            return "Stock_Adjustment" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string ProductionsExcelFilename()
        {
            return "Productions" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }

        public static string SuppliesUsingExcelFilename()
        {
            return "Supplies_Using" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + UniqueIdGenerator.Generate(4, 4) + ".xls";
        }
    }

    public static class UniqueIdGenerator
    {
        // Define default min and max password lengths.
        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private const string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private const string PASSWORD_CHARS_NUMERIC = "123456789";

        public static string Generate(int minLength = 8, int maxLength = 8)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            var charGroups = new[]
            {
                PASSWORD_CHARS_UCASE.ToCharArray(),
                PASSWORD_CHARS_NUMERIC.ToCharArray()
            };

            // Use this array to track the number of unused characters in each
            // character group.
            var charsLeftInGroup = new int[charGroups.Length];
            // Initially, all characters in each group are not used.
            for (var i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;
            // Use this array to track (iterate through) unused character groups.
            var leftGroupsOrder = new int[charGroups.Length];
            // Initially, all character groups are not used.
            for (var i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            var randomBytes = new byte[4];
            // Generate 4 random bytes.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            // Convert 4 bytes into a 32-bit integer value.
            var seed = (randomBytes[0] & 0x7f) << 24 |
                        randomBytes[1] << 16 |
                        randomBytes[2] << 8 |
                        randomBytes[3];

            // Now, this is real randomization.
            var random = new Random(seed);
            // This array will hold password characters.
            // Allocate appropriate memory for the password.
            var password = minLength < maxLength ? new char[random.Next(minLength, maxLength + 1)] : new char[minLength];
            var lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
            // Generate password characters one at a time.
            for (var i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                var nextLeftGroupsOrderIdx = lastLeftGroupsOrderIdx == 0 ? 0 : random.Next(0, lastLeftGroupsOrderIdx);
                // Get the actual index of the character group, from which we will
                // pick the next character.
                var nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];
                // Get the index of the last unprocessed characters in this group.
                var lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;
                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                var nextCharIdx = lastCharIdx == 0 ? 0 : random.Next(0, lastCharIdx + 1);
                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];
                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        var temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }
            // Convert password characters into a string and return the result.
            return new string(password);
        }
    }
}
