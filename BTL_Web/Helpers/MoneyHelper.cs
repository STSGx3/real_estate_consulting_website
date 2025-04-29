using BTL_Web;
namespace BTL_Web.Helpers
{
    public static class MoneyHelper
    {
        public static string FormatMoney(decimal amount)
        {
            string formattedAmount = string.Empty;

            if (amount >= 1_000_000_000)
            {
                formattedAmount = (amount / 1_000_000_000).ToString("#,0") + " tỷ";
            }
            else if (amount >= 1_000_000)
            {
                formattedAmount = (amount / 1_000_000).ToString("#,0") + " triệu";
            }
            else if (amount >= 1_000)
            {
                formattedAmount = (amount / 1_000).ToString("#,0") + " nghìn";
            }
            else
            {
                formattedAmount = amount.ToString("#,0");
            }

            // Chuyển đổi tiền từ dạng số sang dạng rút gọn theo đơn vị VN 
            return formattedAmount + " VND";
        }

    }
}
