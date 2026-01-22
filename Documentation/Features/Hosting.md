# HostingPanelLib `IHostingPanel` Interface - User Manual

The `IHostingPanel` interface provides a standardized way to manage web hosting accounts, email accounts, and databases programmatically. This interface is asynchronous and designed for integration with hosting management systems.

---

## Table of Contents

1. [Web Hosting Accounts](#web-hosting-accounts)
    - Create
    - Update
    - Suspend
    - Unsuspend
    - Delete
    - Get Info
    - List All
2. [Email Accounts](#email-accounts)
    - Create
    - Update
    - Delete
    - Get Info
    - List All
    - Change Password
3. [Account Resource Management](#account-resource-management)
    - Set Disk Quota
    - Set Bandwidth Limit
4. [Databases](#databases)
    - Create Database
    - Delete Database
    - Get Info
    - List All
5. [Database Users](#database-users)
    - Create User
    - Delete User
    - Grant Privileges
    - Change Password

---

## Web Hosting Accounts

### `CreateWebHostingAccountAsync(HostingAccountRequest request)`
Creates a new web hosting account.

- **Parameters:**
  - `request` (`HostingAccountRequest`): Details of the account to create (e.g., domain, username, plan).
- **Returns:** `Task<HostingAccountResult>` — Contains account ID, status, and any errors.

---

### `UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request)`
Updates an existing web hosting account.

- **Parameters:**
  - `accountId` (`string`): ID of the account to update.
  - `request` (`HostingAccountRequest`): Updated account details.
- **Returns:** `Task<AccountUpdateResult>` — Indicates success or failure of the operation.

---

### `SuspendWebHostingAccountAsync(string accountId)`
Suspends a web hosting account.

- **Parameters:**
  - `accountId` (`string`): ID of the account to suspend.
- **Returns:** `Task<AccountUpdateResult>`

---

### `UnsuspendWebHostingAccountAsync(string accountId)`
Unsuspends a suspended web hosting account.

- **Parameters:**
  - `accountId` (`string`): ID of the account to unsuspend.
- **Returns:** `Task<AccountUpdateResult>`

---

### `DeleteWebHostingAccountAsync(string accountId)`
Deletes a web hosting account.

- **Parameters:**
  - `accountId` (`string`): ID of the account to delete.
- **Returns:** `Task<AccountUpdateResult>`

---

### `GetWebHostingAccountInfoAsync(string accountId)`
Retrieves information about a web hosting account.

- **Parameters:**
  - `accountId` (`string`): ID of the account.
- **Returns:** `Task<AccountInfoResult>` — Contains account details.

---

### `ListWebHostingAccountsAsync()`
Lists all web hosting accounts.

- **Returns:** `Task<List<AccountInfoResult>>` — Collection of all hosting accounts.

---

## Email Accounts

### `CreateMailAccountAsync(MailAccountRequest request)`
Creates a new email account.

- **Parameters:**
  - `request` (`MailAccountRequest`): Details of the email account (e.g., address, password, domain).
- **Returns:** `Task<MailAccountResult>`

---

### `UpdateMailAccountAsync(string accountId, MailAccountRequest request)`
Updates an existing email account.

- **Parameters:**
  - `accountId` (`string`): Email account ID.
  - `request` (`MailAccountRequest`): Updated details.
- **Returns:** `Task<AccountUpdateResult>`

---

### `DeleteMailAccountAsync(string accountId)`
Deletes an email account.

- **Parameters:**
  - `accountId` (`string`): ID of the email account.
- **Returns:** `Task<AccountUpdateResult>`

---

### `GetMailAccountInfoAsync(string accountId)`
Gets information about an email account.

- **Parameters:**
  - `accountId` (`string`): ID of the email account.
- **Returns:** `Task<AccountInfoResult>`

---

### `ListMailAccountsAsync(string domain)`
Lists all email accounts for a domain.

- **Parameters:**
  - `domain` (`string`): Domain name to list accounts for.
- **Returns:** `Task<List<AccountInfoResult>>`

---

### `ChangeMailPasswordAsync(string accountId, string newPassword)`
Changes the password for an email account.

- **Parameters:**
  - `accountId` (`string`): ID of the email account.
  - `newPassword` (`string`): New password.
- **Returns:** `Task<AccountUpdateResult>`

---

## Account Resource Management

### `SetDiskQuotaAsync(string accountId, int quotaMB)`
Sets the disk quota for a hosting account.

- **Parameters:**
  - `accountId` (`string`): Account ID.
  - `quotaMB` (`int`): Disk quota in MB.
- **Returns:** `Task<AccountUpdateResult>`

---

### `SetBandwidthLimitAsync(string accountId, int bandwidthMB)`
Sets the bandwidth limit for a hosting account.

- **Parameters:**
  - `accountId` (`string`): Account ID.
  - `bandwidthMB` (`int`): Bandwidth limit in MB.
- **Returns:** `Task<AccountUpdateResult>`

---

## Databases

### `CreateDatabaseAsync(DatabaseRequest request)`
Creates a new database.

- **Parameters:**
  - `request` (`DatabaseRequest`): Database creation details (name, user, password).
- **Returns:** `Task<DatabaseResult>`

---

### `DeleteDatabaseAsync(string databaseId)`
Deletes a database.

- **Parameters:**
  - `databaseId` (`string`): Database ID.
- **Returns:** `Task<AccountUpdateResult>`

---

### `GetDatabaseInfoAsync(string databaseId)`
Retrieves information about a database.

- **Parameters:**
  - `databaseId` (`string`): Database ID.
- **Returns:** `Task<AccountInfoResult>`

---

### `ListDatabasesAsync(string domain)`
Lists all databases for a domain.

- **Parameters:**
  - `domain` (`string`): Domain name.
- **Returns:** `Task<List<AccountInfoResult>>`

---

## Database Users

### `CreateDatabaseUserAsync(DatabaseUserRequest request)`
Creates a new database user.

- **Parameters:**
  - `request` (`DatabaseUserRequest`): User details.
- **Returns:** `Task<DatabaseResult>`

---

### `DeleteDatabaseUserAsync(string userId)`
Deletes a database user.

- **Parameters:**
  - `userId` (`string`): User ID.
- **Returns:** `Task<AccountUpdateResult>`

---

### `GrantDatabasePrivilegesAsync(string userId, string databaseId, List<string> privileges)`
Grants privileges to a database user on a specific database.

- **Parameters:**
  - `userId` (`string`): User ID.
  - `databaseId` (`string`): Database ID.
  - `privileges` (`List<string>`): Privileges to grant (e.g., SELECT, INSERT, UPDATE).
- **Returns:** `Task<AccountUpdateResult>`

---

### `ChangeDatabasePasswordAsync(string userId, string newPassword)`
Changes the password for a database user.

- **Parameters:**
  - `userId` (`string`): Database user ID.
  - `newPassword` (`string`): New password.
- **Returns:** `Task<AccountUpdateResult>`

---

## Notes

- All methods are asynchronous and should be awaited.
- All operations return result objects containing success/failure status and error messages where applicable.
- IDs (`accountId`, `databaseId`, `userId`) are returned from the creation methods and must be stored for further operations.
