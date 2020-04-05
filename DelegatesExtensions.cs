using System;

namespace ConcurrencyWizardLibrary
{
    public static class DelegatesExtensions
    {
        public static Func<A, C> FromFuncComposeHof<A, B, C >
            (this Func<A, B> f, Func<B, C> g)
        {
            return n => g(f(n));   
        }        
    }


}
