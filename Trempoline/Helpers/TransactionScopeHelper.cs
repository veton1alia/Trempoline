using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace Trempoline.Helpers
{
    public static class TransactionScopeHelper
    {

        public static TransactionScope Scope 
        { 
            get
            {
                return  new TransactionScope(
                            TransactionScopeOption.RequiresNew, // a new transactions will always be created
                            new TransactionOptions()
                            {
                                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted //no need to support concurrent transactions. 
                            }
                        );
            }
        }

    }
}