using PackageDelivery.Delivery;
using PackageDeliveryNew.Utils;

namespace PackageDelivery
{
    public partial class App
    {
        public App()
        {
            string legacyConnectionString =
                "Server=localhost;Database=PackageDelivery;User Id=packageDelivery;Password=pd;TrustServerCertificate=True;";

            string bubbleConnectionString =
                "Server=localhost;Database=PackageDeliveryNew;User Id=packageDelivery;Password=pd;TrustServerCertificate=True;";

            DBHelper.Init(legacyConnectionString);
            ConnectionString.Set(bubbleConnectionString);
        }
    }
}
