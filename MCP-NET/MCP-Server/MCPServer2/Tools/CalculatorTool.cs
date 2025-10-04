using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Tools
{
    [McpServerToolType]
    public class CalculatorTool
    {
        [McpServerTool, Description("This function will add two numbers.")]
        public static int Addition(
            [Description ("First number")] int firstNumber,
            [Description("Second number")] int secondNumber)
        {
            return firstNumber + secondNumber;
        }

        [McpServerTool, Description("This function will subtract three numbers.")]
        public static int Subtraction(
            [Description("First number")] int firstNumber,
            [Description("Second number")] int secondNumber,
            [Description("Third number")] int thirdNumber)
        {
            return firstNumber - secondNumber - thirdNumber;
        }
    }
}