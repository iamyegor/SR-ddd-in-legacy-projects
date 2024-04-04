using PackageDelivery.Delivery;
using PackageDeliveryNew.Utils;

namespace PackageDelivery
{
    public partial class App
    {
        public App()
        {
            string legacyConnectionString =
                "Server=localhost;Database=PackageDelivery;User Id=packageDelivery;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

            string bubbleConnectionString =
                "Host=localhost;Port=5432;Username=postgres;Password=yegor;Database=sr_package_delivery_new";

            DBHelper.Init(legacyConnectionString);
            ConnectionString.Set(bubbleConnectionString);
        }
    }
}
