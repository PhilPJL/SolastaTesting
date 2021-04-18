using JetBrains.Annotations;
using System;

namespace SolastaTesting.Infrastructure
{
    public class SetResetToken : Disposable
    {
        private Action _reset;

        public SetResetToken([NotNull] Action set, [NotNull] Action reset)
        {
            Preconditions.IsNotNull(set, nameof(set));
            Preconditions.IsNotNull(reset, nameof(reset));

            _reset = reset;

            set();
        }

        protected override void Dispose(bool disposing)
        {
            if (_reset != null)
            {
                _reset();
                _reset = null;
            }
        }
    }
}
