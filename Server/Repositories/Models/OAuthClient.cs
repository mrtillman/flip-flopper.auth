using System;

namespace CounterCulture.Repositories.Models {
    public class OAuthClient {
      public string app_type { get; set; }
      public string app_name { get; set; }
      public string app_description { get; set; }
      public string client_id { get; set; }
      public string client_secret { get; set; }
      public string homepage_uri { get; set; }
      public string redirect_uri { get; set; }
      public string grant_types { get; set; }
      public string scope { get; set; }
      public string user_id { get; set; }
    }
}