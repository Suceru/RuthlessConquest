using System;
using System.Threading;

namespace Game
{
    public class CancellableProgress : Progress
    {
        public event Action Cancelled;

        public CancellableProgress()
        {
            this.CancellationToken = this.CancellationTokenSource.Token;
        }

        public void Cancel()
        {
            this.CancellationTokenSource.Cancel();
            Action cancelled = this.Cancelled;
            if (cancelled == null)
            {
                return;
            }
            cancelled();
        }

        public readonly CancellationToken CancellationToken;

        private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    }
}
