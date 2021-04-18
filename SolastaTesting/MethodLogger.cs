using SolastaTesting.Infrastructure;

namespace SolastaTesting
{
    public class MethodLogger : SetResetToken
    {
        public MethodLogger(string methodName) : base(() => Main.Log($"{methodName}: Enter"), () => Main.Log($"{methodName}: Leave"))
        {
        }
    }
}
