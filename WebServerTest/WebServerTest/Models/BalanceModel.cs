using System;
using System.Collections.Generic;

namespace WebServerTest.Models
{
    public class BalanceModel
    {
        IDictionary<string, int> balanceData = new Dictionary<string, int>();

        public BalanceModel()
        {

        }

        public int AddAmount(string accountId, int amount)
        {
            if (!balanceData.ContainsKey(accountId))
            {
                balanceData[accountId] = 0;
            }

            balanceData[accountId] += amount;
            return balanceData[accountId];
        }

        public int? GetAmount(string accountId)
        {
            if (balanceData.ContainsKey(accountId))
            {
                return balanceData[accountId];
            }

            return null;
        }
    }
}
