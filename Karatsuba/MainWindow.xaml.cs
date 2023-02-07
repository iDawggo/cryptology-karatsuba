using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Karatsuba
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            productOut.Text = "";
            errorsOut.Text = "";

            //Unformatting text, restricting the string to only numbers.
            String unformatted1 = Regex.Replace(input1.Text, "[^0-9]", "");
            String unformatted2 = Regex.Replace(input2.Text, "[^0-9]", "");

            //Error-checking input.
            if (unformatted1 == String.Empty || unformatted2 == String.Empty)
            {
                errorsOut.Text = "PLEASE ENTER A CORRECT (INTEGER!!!) INPUT FOR EACH BOX.";
                return;
            }

            //Creating a 2D array for stacking the two large inputs, using a method to do so.
            String[,] largeStack = stackNumbers(unformatted1, unformatted2);

            //Using my big multiplication algorithm, taking the same inputs as big addition.
            productOut.Text = karatsuba(unformatted1, unformatted2);
        }

        public String largeInputNum;
        public String smallInputNum;

        bool num1IsSmall (String num1, String num2)
        {
            if (num1.Length < num2.Length)
            {
                return true;
            }
            if (num1.Length > num2.Length)
            {
                return false;
            }

            char[] array1 = num1.ToCharArray();
            char[] array2 = num2.ToCharArray();

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] < array2[i])
                {
                    return true;
                }
                if (array1[i] > array2[i])
                {
                    return false;
                }
            }
            return false;
        }

        String[,] stackNumbers (String number1, String number2)
        {
            /*
             * My method for stacking two large numbers into a large two row 2D array to make it easier to add or multiply.
             * 
             * Looks for the largest number overall as it would dominate the amount of digits, making it the max length of the array.
             * Loops each character of the large number into each array space in the first row.
             * Pads the smaller number to the left with zeroes so its digits would align with the larger number at the end.
             * Loops each character of the small number into each array space in the second row, following the padding.
             * Returns the 2D array.
             */

            String largerNum;
            String smallerNum;
            int largeNumLength;
            int smallNumLength;

            //Determines if a larger than another, storing them respecively in largerNum or smallerNum.
            if (num1IsSmall(number1, number2))
            {
                smallerNum = number1;
                smallInputNum = number1;
                largerNum = number2;
                largeInputNum = number2;
            }
            else
            {
                smallerNum = number2;
                smallInputNum = number2;
                largerNum = number1;
                largeInputNum = number1;
            }

            //Calculating lengths of the smaller and larger num.
            largeNumLength = largerNum.Length;
            smallNumLength = smallerNum.Length;

            //Creating a 2D array of the stack of the larger and smaller number, with a row length according to the larger number's length.
            String[,] numberStack = new String[2, largeNumLength];

            //Process for padding the smaller number to line up with the larger number's digits.
            if (largeNumLength - smallNumLength != 0)
            {
                for (int i = 0; i < (largeNumLength - smallNumLength); i++)
                {
                    smallerNum = "0" + smallerNum;
                }
            }

            //Adding each digit of the larger number into the first row of the 2D array, and the smaller number in the second row.
            for (int i = 0; i < largeNumLength; i++)
            {
                numberStack[0, i] = largerNum.Substring(i, 1);
                numberStack[1, i] = smallerNum.Substring(i, 1);
            }

            return numberStack;
        }

        String bigAddition (String[,] stack, int rowLength)
        {
            /*
             * My method for adding two extremely large numbers together.
             * 
             * Basic rundown:
             * Using the stacked 2D array to add each digit together.
             * Ensuring extra digits are carried over to the next digit.
             */

            String result = "";
            int tempCarry = 0;

            //Traversing throughout the entire row length of the array (aka, the amount of digits in the number)
            for (int i = rowLength - 1; i >= 0; i--)
            {
                int tempInt1; //Temp int variables
                int tempInt2; //for adding each digit together.

                Int32.TryParse(stack[0, i], out tempInt1); //Actually parsing each character
                Int32.TryParse(stack[1, i], out tempInt2); //from the array into a string.

                int sum = tempInt1 + tempInt2; //Calculating the sum of the digits.


                if ((sum % 10) + tempCarry >= 10) //Provison if a sum's one place plus a carry is or is over ten, needing another carry.
                {
                    sum = ((sum % 10) + tempCarry) % 10; //Calculates the sum of the sum and carry mod ten for the new ones place.
                    tempCarry += ((sum % 10) + tempCarry) / 10; //Calculates the new carry, appending it to the currrent carry.
                }
                else
                {
                    sum = (sum % 10) + tempCarry; //Adding the ones place of the sum (guaranteed by mod 10) and a possible carry number.
                    tempCarry = (tempInt1 + tempInt2) / 10; //Calculates a carry number with the sum divided by 10 to get the tens place, storing it in tempCarry.
                }

                result = sum.ToString() + result;
            }

            if (tempCarry > 0) //Provision for when the sum of two numbers result in a higher place.
            {
                result = tempCarry.ToString() + result; //Adding the remaining carry to the result.
            }

            return result;
        }

        String bigSubtract (String[,] stack, int rowLength)
        {
            String result = "";
            int tempCarry = 0;
            bool isNegative = false;

            if(num1IsSmall(largeInputNum, smallInputNum))
            {
                isNegative = true;
            }

            for (int i = rowLength - 1; i >= 0; i--)
            {
                int tempInt1;
                int tempInt2;
                bool addCarry = false;

                Int32.TryParse(stack[0, i], out tempInt1); 
                Int32.TryParse(stack[1, i], out tempInt2);

                if (tempInt1 - tempCarry < tempInt2)
                {
                    tempInt1 += 10;
                    addCarry = true;
                }

                int difference = tempInt1 - tempInt2 - tempCarry;

                if (addCarry)
                {
                    tempCarry = 1;
                }
                else
                {
                    tempCarry = 0;
                }

                result = difference.ToString() + result;
            }

            if (tempCarry > 0)
            {
                result = tempCarry.ToString() + result;
            }

            if (isNegative)
            {
                result = "-" + result;
            }

            return result;
        }

        int splitPosition (String num1, String num2)
        {
            int smallerNum = Math.Min(num1.Length, num2.Length);

            if (smallerNum % 2 == 1)
            {
                return (smallerNum + 1) / 2;
            }
            else
            {
                return smallerNum / 2;
            }
        }

        String firstSplit (String num, int split)
        {
            return num.Remove(num.Length - split);
        }

        String secondSplit (String num, int split)
        {
            return num.Substring(num.Length - split);
        }

        String finalCalculation (String ac, String bd, String prodABCD, int n)
        {
            String[,] abcd_acStack = stackNumbers(prodABCD, ac);
            String[,] subtract_bdStack = stackNumbers(bigSubtract(abcd_acStack, (abcd_acStack.Length / 2)), bd);

            String part1 = bigSubtract(subtract_bdStack, (subtract_bdStack.Length / 2));
            String part2 = part1.PadRight(part1.Length + (n / 2), '0');
            String part3 = ac.PadRight(ac.Length + n, '0');

            String[,] sumStack1 = stackNumbers(part2, part3);
            String[,] sumStack2 = stackNumbers(bigAddition(sumStack1, (sumStack1.Length / 2)), bd);

            String finalCalc = bigAddition(sumStack2, (sumStack2.Length / 2));

            return finalCalc;
        }

        String karatsuba (String num1, String num2)
        {
            String result;
            int split = splitPosition(num1, num2);

            if (num1.Length == 1 || num2.Length == 1)
            {
                return (Int32.Parse(num1) * Int32.Parse(num2)).ToString();
            }

            String varA = firstSplit(num1, split);
            String varB = secondSplit(num1, split);
            String varC = firstSplit(num2, split);
            String varD = secondSplit(num2, split);

            String prodAC = karatsuba(varA, varC);
            String prodBD = karatsuba(varB, varD);

            String[,] abStack = stackNumbers(varA, varB);
            String[,] cdStack = stackNumbers(varC, varD);
            String prodABCD = karatsuba(bigAddition(abStack, (abStack.Length / 2)), bigAddition(cdStack, (cdStack.Length / 2)));

            result = finalCalculation(prodAC, prodBD, prodABCD, (varB.Length + varD.Length));

            return result;
        }
    }
}
