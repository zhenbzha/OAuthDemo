using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using OAuthDemo.Models;
using System.Linq;

namespace OAuthDemo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private const string ScopeRead = "product.read";
        private const string ScopeUserImpersonation = "user_impersonation";

        private IHttpContextAccessor httpContextAccessor;

        private AuthenticationSettings authSettings { get; set; }

        public ProductController(IHttpContextAccessor httpContextAcc, IOptions<AuthenticationSettings> settings)
        {
            httpContextAccessor = httpContextAcc;
            authSettings = settings.Value;
        }

        // GET: api/product
        [HttpGet]
        public IActionResult Get()
        {
            var scopes = HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope")?.Value;
            if (!(!string.IsNullOrEmpty(ScopeRead) && scopes != null
                    && scopes.Split(' ').Any(s => s.Equals(ScopeRead) || s.Equals(ScopeUserImpersonation))))
                return Unauthorized();

            JsonResult retVal = null;

            AuthenticationResult authResult = AuthenticationHelper.GetAuthenticationResult(httpContextAccessor, authSettings);

            if (authResult != null)
            {
                string queryString = "SELECT TOP (1000) [Product], [SalesRep] FROM[dbo].[Sales]";

                using (SqlConnection connection = new SqlConnection(authSettings.ConnectionString))
                {
                    connection.AccessToken = authResult.AccessToken;
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(queryString, connection);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);

                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        var products = new List<Product>();
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            var productName = table.Rows[i]["Product"].ToString();
                            var salesRep = table.Rows[i]["SalesRep"].ToString();

                            products.Add(new Product { Name = productName, SalesRep = salesRep });
                        }

                        retVal = new JsonResult(products);
                    }
                    catch (SqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return Ok(retVal);
        }
    }
}
