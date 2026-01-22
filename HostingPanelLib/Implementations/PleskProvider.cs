using HostingPanelLib.Models;
using System.Text;
using System.Xml.Linq;

namespace HostingPanelLib.Implementations
{
    public class PleskProvider : BaseHostingPanel
    {
        private readonly string _apiKey;
        private readonly string? _username;
        private readonly string? _password;
        private readonly int _port;
        private readonly bool _useHttps;

        public PleskProvider(string apiUrl, string apiKey, string? username, string? password, int port, bool useHttps)
            : base(apiUrl)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _username = username;
            _password = password;
            _port = port;
            _useHttps = useHttps;

            _httpClient.DefaultRequestHeaders.Add("HTTP_AUTH_LOGIN", _username ?? "admin");
            _httpClient.DefaultRequestHeaders.Add("HTTP_AUTH_PASSWD", _password ?? _apiKey);
            _httpClient.DefaultRequestHeaders.Add("HTTP_PRETTY_PRINT", "TRUE");
        }

        private async Task<XDocument?> SendPleskRequestAsync(XElement packetContent)
        {
            try
            {
                var packet = new XElement("packet",
                    new XAttribute("version", "1.6.9.0"),
                    packetContent
                );

                var xmlContent = new StringContent(
                    $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>{packet}",
                    Encoding.UTF8,
                    "text/xml"
                );

                var response = await _httpClient.PostAsync("/enterprise/control/agent.php", xmlContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return XDocument.Parse(responseContent);
            }
            catch
            {
                return null;
            }
        }

        private bool IsSuccess(XElement? result)
        {
            return result?.Element("status")?.Value == "ok";
        }

        private string? GetErrorMessage(XElement? result)
        {
            return result?.Element("errtext")?.Value ?? result?.Element("errcode")?.Value;
        }

        public override async Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Domain))
                {
                    return CreateHostingErrorResult("Domain name is required", "INVALID_DOMAIN");
                }

                var webspaceCreate = new XElement("webspace",
                    new XElement("add",
                        new XElement("gen_setup",
                            new XElement("name", request.Domain),
                            new XElement("ip_address", "shared"),
                            new XElement("htype", "vrt_hst")
                        ),
                        new XElement("hosting",
                            new XElement("vrt_hst",
                                new XElement("property",
                                    new XElement("name", "ftp_login"),
                                    new XElement("value", request.Username)
                                ),
                                new XElement("property",
                                    new XElement("name", "ftp_password"),
                                    new XElement("value", request.Password)
                                ),
                                request.DiskQuotaMB.HasValue ? new XElement("property",
                                    new XElement("name", "disk_space"),
                                    new XElement("value", (request.DiskQuotaMB.Value * 1024).ToString())
                                ) : null
                            )
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceCreate);
                var result = response?.Root?.Element("webspace")?.Element("add")?.Element("result");

                if (IsSuccess(result))
                {
                    var siteId = result?.Element("id")?.Value;
                    return new HostingAccountResult
                    {
                        Success = true,
                        Message = "Webspace created successfully",
                        AccountId = siteId ?? request.Domain,
                        Domain = request.Domain,
                        Username = request.Username,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateHostingErrorResult(GetErrorMessage(result) ?? "Failed to create webspace", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateHostingErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                var webspaceSet = new XElement("webspace",
                    new XElement("set",
                        new XElement("filter",
                            new XElement("name", accountId)
                        ),
                        new XElement("values",
                            request.DiskQuotaMB.HasValue ? new XElement("limits",
                                new XElement("disk_space", (request.DiskQuotaMB.Value * 1024).ToString())
                            ) : null
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceSet);
                var result = response?.Root?.Element("webspace")?.Element("set")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Webspace updated successfully",
                        AccountId = accountId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to update webspace", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                var webspaceSet = new XElement("webspace",
                    new XElement("set",
                        new XElement("filter",
                            new XElement("name", accountId)
                        ),
                        new XElement("values",
                            new XElement("gen_setup",
                                new XElement("status", "16") // 16 = suspended
                            )
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceSet);
                var result = response?.Root?.Element("webspace")?.Element("set")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Webspace suspended successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        NewValue = "Suspended",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to suspend webspace", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                var webspaceSet = new XElement("webspace",
                    new XElement("set",
                        new XElement("filter",
                            new XElement("name", accountId)
                        ),
                        new XElement("values",
                            new XElement("gen_setup",
                                new XElement("status", "0") // 0 = active
                            )
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceSet);
                var result = response?.Root?.Element("webspace")?.Element("set")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Webspace unsuspended successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        NewValue = "Active",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to unsuspend webspace", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                var webspaceDelete = new XElement("webspace",
                    new XElement("del",
                        new XElement("filter",
                            new XElement("name", accountId)
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceDelete);
                var result = response?.Root?.Element("webspace")?.Element("del")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Webspace deleted successfully",
                        AccountId = accountId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to delete webspace", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateInfoErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                var webspaceGet = new XElement("webspace",
                    new XElement("get",
                        new XElement("filter",
                            new XElement("name", accountId)
                        ),
                        new XElement("dataset",
                            new XElement("gen_info"),
                            new XElement("limits")
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceGet);
                var result = response?.Root?.Element("webspace")?.Element("get")?.Element("result");

                if (IsSuccess(result))
                {
                    var data = result?.Element("data");
                    var genInfo = data?.Element("gen_info");
                    var limits = data?.Element("limits");

                    return new AccountInfoResult
                    {
                        Success = true,
                        AccountId = genInfo?.Element("id")?.Value ?? accountId,
                        Domain = genInfo?.Element("name")?.Value,
                        Status = genInfo?.Element("status")?.Value == "0" ? "Active" : "Suspended",
                        DiskQuotaMB = limits?.Element("disk_space")?.Value != null
                            ? int.Parse(limits.Element("disk_space").Value) / 1024
                            : null,
                        IpAddress = genInfo?.Element("real_ip")?.Value
                    };
                }

                return CreateInfoErrorResult(GetErrorMessage(result) ?? "Webspace not found", "NOT_FOUND");
            }
            catch (Exception ex)
            {
                return CreateInfoErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<List<AccountInfoResult>> ListWebHostingAccountsAsync()
        {
            try
            {
                var webspaceGet = new XElement("webspace",
                    new XElement("get",
                        new XElement("filter"),
                        new XElement("dataset",
                            new XElement("gen_info")
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceGet);
                var results = response?.Root?.Element("webspace")?.Element("get")?.Elements("result");

                if (results != null)
                {
                    return results.Where(r => IsSuccess(r))
                        .Select(r =>
                        {
                            var genInfo = r.Element("data")?.Element("gen_info");
                            return new AccountInfoResult
                            {
                                Success = true,
                                AccountId = genInfo?.Element("id")?.Value ?? "",
                                Domain = genInfo?.Element("name")?.Value,
                                Status = genInfo?.Element("status")?.Value == "0" ? "Active" : "Suspended"
                            };
                        }).ToList();
                }

                return new List<AccountInfoResult>();
            }
            catch
            {
                return new List<AccountInfoResult>();
            }
        }

        public override async Task<MailAccountResult> CreateMailAccountAsync(MailAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.EmailAddress) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return CreateMailErrorResult("Email address and password are required", "INVALID_INPUT");
                }

                var mailCreate = new XElement("mail",
                    new XElement("create",
                        new XElement("filter",
                            new XElement("site-name", request.Domain)
                        ),
                        new XElement("mailname",
                            new XElement("name", request.EmailAddress.Split('@')[0]),
                            new XElement("mailbox",
                                new XElement("enabled", "true"),
                                new XElement("quota", (request.QuotaMB ?? 100) * 1024 * 1024)
                            ),
                            new XElement("password",
                                new XElement("value", request.Password),
                                new XElement("type", "plain")
                            )
                        )
                    )
                );

                var response = await SendPleskRequestAsync(mailCreate);
                var result = response?.Root?.Element("mail")?.Element("create")?.Element("result");

                if (IsSuccess(result))
                {
                    return new MailAccountResult
                    {
                        Success = true,
                        Message = "Email account created successfully",
                        EmailAddress = request.EmailAddress,
                        Domain = request.Domain,
                        QuotaMB = request.QuotaMB ?? 100,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateMailErrorResult(GetErrorMessage(result) ?? "Failed to create email", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateMailErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request)
        {
            try
            {
                var emailParts = accountId.Split('@');
                var mailUpdate = new XElement("mail",
                    new XElement("update",
                        new XElement("set",
                            new XElement("filter",
                                new XElement("site-name", emailParts[1])
                            ),
                            new XElement("mailname",
                                new XElement("name", emailParts[0]),
                                new XElement("mailbox",
                                    new XElement("quota", (request.QuotaMB ?? 100) * 1024 * 1024)
                                )
                            )
                        )
                    )
                );

                var response = await SendPleskRequestAsync(mailUpdate);
                var result = response?.Root?.Element("mail")?.Element("update")?.Element("set")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult { Success = true, Message = "Email updated", AccountId = accountId, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to update email", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId)
        {
            try
            {
                var emailParts = accountId.Split('@');
                var mailDelete = new XElement("mail",
                    new XElement("remove",
                        new XElement("filter",
                            new XElement("site-name", emailParts[1])
                        ),
                        new XElement("name", emailParts[0])
                    )
                );

                var response = await SendPleskRequestAsync(mailDelete);
                var result = response?.Root?.Element("mail")?.Element("remove")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult { Success = true, Message = "Email deleted", AccountId = accountId, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to delete email", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId)
        {
            try
            {
                var emailParts = accountId.Split('@');
                return new AccountInfoResult { Success = true, AccountId = accountId, Email = accountId, Domain = emailParts[1] };
            }
            catch (Exception ex)
            {
                return CreateInfoErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain)
        {
            try
            {
                return new List<AccountInfoResult>();
            }
            catch
            {
                return new List<AccountInfoResult>();
            }
        }

        public override async Task<AccountUpdateResult> ChangeMailPasswordAsync(string accountId, string newPassword)
        {
            try
            {
                var emailParts = accountId.Split('@');
                var mailUpdate = new XElement("mail",
                    new XElement("update",
                        new XElement("set",
                            new XElement("filter",
                                new XElement("site-name", emailParts[1])
                            ),
                            new XElement("mailname",
                                new XElement("name", emailParts[0]),
                                new XElement("password",
                                    new XElement("value", newPassword),
                                    new XElement("type", "plain")
                                )
                            )
                        )
                    )
                );

                var response = await SendPleskRequestAsync(mailUpdate);
                var result = response?.Root?.Element("mail")?.Element("update")?.Element("set")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult { Success = true, Message = "Password changed", AccountId = accountId, UpdatedField = "Password", UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to change password", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB)
        {
            try
            {
                var webspaceSet = new XElement("webspace",
                    new XElement("set",
                        new XElement("filter", new XElement("name", accountId)),
                        new XElement("values",
                            new XElement("limits", new XElement("disk_space", quotaMB * 1024))
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceSet);
                var result = response?.Root?.Element("webspace")?.Element("set")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult { Success = true, Message = "Quota updated", AccountId = accountId, UpdatedField = "DiskQuota", NewValue = quotaMB, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to set quota", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB)
        {
            try
            {
                var webspaceSet = new XElement("webspace",
                    new XElement("set",
                        new XElement("filter", new XElement("name", accountId)),
                        new XElement("values",
                            new XElement("limits", new XElement("max_traffic", bandwidthMB))
                        )
                    )
                );

                var response = await SendPleskRequestAsync(webspaceSet);
                var result = response?.Root?.Element("webspace")?.Element("set")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult { Success = true, Message = "Bandwidth updated", AccountId = accountId, UpdatedField = "Bandwidth", NewValue = bandwidthMB, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to set bandwidth", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<DatabaseResult> CreateDatabaseAsync(DatabaseRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.DatabaseName) || string.IsNullOrWhiteSpace(request.Domain))
                {
                    return CreateDatabaseErrorResult("Database name and domain are required", "INVALID_INPUT");
                }

                var dbCreate = new XElement("database",
                    new XElement("add-db",
                        new XElement("webspace-name", request.Domain),
                        new XElement("name", request.DatabaseName),
                        new XElement("type", request.DatabaseType ?? "mysql")
                    )
                );

                var response = await SendPleskRequestAsync(dbCreate);
                var result = response?.Root?.Element("database")?.Element("add-db")?.Element("result");

                if (IsSuccess(result))
                {
                    var dbId = result?.Element("id")?.Value;
                    return new DatabaseResult
                    {
                        Success = true,
                        Message = "Database created successfully",
                        DatabaseId = dbId,
                        DatabaseName = request.DatabaseName,
                        DatabaseType = request.DatabaseType ?? "mysql",
                        Server = "localhost",
                        Port = 3306,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateDatabaseErrorResult(GetErrorMessage(result) ?? "Failed to create database", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateDatabaseErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> DeleteDatabaseAsync(string databaseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(databaseId))
                {
                    return CreateUpdateErrorResult("Database ID is required", "INVALID_DATABASE_ID");
                }

                var dbDelete = new XElement("database",
                    new XElement("del-db",
                        new XElement("filter",
                            new XElement("id", databaseId)
                        )
                    )
                );

                var response = await SendPleskRequestAsync(dbDelete);
                var result = response?.Root?.Element("database")?.Element("del-db")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult { Success = true, Message = "Database deleted", AccountId = databaseId, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to delete database", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountInfoResult> GetDatabaseInfoAsync(string databaseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(databaseId))
                {
                    return CreateInfoErrorResult("Database ID is required", "INVALID_DATABASE_ID");
                }

                var dbGet = new XElement("database",
                    new XElement("get-db",
                        new XElement("filter",
                            new XElement("id", databaseId)
                        )
                    )
                );

                var response = await SendPleskRequestAsync(dbGet);
                var result = response?.Root?.Element("database")?.Element("get-db")?.Element("result");

                if (IsSuccess(result))
                {
                    var dbName = result?.Element("name")?.Value;
                    var dbType = result?.Element("type")?.Value;

                    return new AccountInfoResult
                    {
                        Success = true,
                        Message = "Database information retrieved",
                        AccountId = databaseId,
                        DatabaseName = dbName,
                        DatabaseType = dbType
                    };
                }

                return CreateInfoErrorResult(GetErrorMessage(result) ?? "Database not found", "NOT_FOUND");
            }
            catch (Exception ex)
            {
                return CreateInfoErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<List<AccountInfoResult>> ListDatabasesAsync(string domain)
        {
            try
            {
                var dbGet = new XElement("database",
                    new XElement("get-db",
                        new XElement("filter",
                            new XElement("webspace-name", domain)
                        )
                    )
                );

                var response = await SendPleskRequestAsync(dbGet);
                var results = response?.Root?.Element("database")?.Elements("get-db")?.Elements("result");

                if (results != null)
                {
                    return results.Where(r => IsSuccess(r)).Select(r => new AccountInfoResult
                    {
                        Success = true,
                        AccountId = r?.Element("id")?.Value ?? "",
                        DatabaseName = r?.Element("name")?.Value,
                        DatabaseType = r?.Element("type")?.Value
                    }).ToList();
                }

                return new List<AccountInfoResult>();
            }
            catch
            {
                return new List<AccountInfoResult>();
            }
        }

        public override async Task<DatabaseResult> CreateDatabaseUserAsync(DatabaseUserRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return CreateDatabaseErrorResult("Username and password are required", "INVALID_INPUT");
                }

                var dbUserCreate = new XElement("database",
                    new XElement("add-db-user",
                        new XElement("db-id", request.DatabaseName),
                        new XElement("login", request.Username),
                        new XElement("password", request.Password)
                    )
                );

                var response = await SendPleskRequestAsync(dbUserCreate);
                var result = response?.Root?.Element("database")?.Element("add-db-user")?.Element("result");

                if (IsSuccess(result))
                {
                    return new DatabaseResult
                    {
                        Success = true,
                        Message = "Database user created successfully",
                        Username = request.Username,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateDatabaseErrorResult(GetErrorMessage(result) ?? "Failed to create database user", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateDatabaseErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> DeleteDatabaseUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return CreateUpdateErrorResult("User ID is required", "INVALID_USER_ID");
                }

                var dbUserDelete = new XElement("database",
                    new XElement("del-db-user",
                        new XElement("filter",
                            new XElement("id", userId)
                        )
                    )
                );

                var response = await SendPleskRequestAsync(dbUserDelete);
                var result = response?.Root?.Element("database")?.Element("del-db-user")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult { Success = true, Message = "Database user deleted", AccountId = userId, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to delete database user", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> GrantDatabasePrivilegesAsync(string userId, string databaseId, List<string> privileges)
        {
            try
            {
                return new AccountUpdateResult
                {
                    Success = true,
                    Message = "Privileges are granted automatically in Plesk when database user is created",
                    AccountId = userId,
                    UpdatedDate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> ChangeDatabasePasswordAsync(string userId, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return CreateUpdateErrorResult("User ID is required", "INVALID_USER_ID");
                }

                var dbUserUpdate = new XElement("database",
                    new XElement("set-db-user",
                        new XElement("filter",
                            new XElement("id", userId)
                        ),
                        new XElement("values",
                            new XElement("password", newPassword)
                        )
                    )
                );

                var response = await SendPleskRequestAsync(dbUserUpdate);
                var result = response?.Root?.Element("database")?.Element("set-db-user")?.Element("result");

                if (IsSuccess(result))
                {
                    return new AccountUpdateResult { Success = true, Message = "Password changed", AccountId = userId, UpdatedField = "Password", UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(GetErrorMessage(result) ?? "Failed to change password", "PLESK_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }
    }
}
