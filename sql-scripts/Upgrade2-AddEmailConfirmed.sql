ALTER TABLE dbo.GlobomanticsUser
ADD [EmailConfirmed] BIT NULL

-- Run the statement below if you want to enable existing accounts to login
-- and not explicitly verify email
--UPDATE dbo.GlobomanticsUser SET EmailConfirmed = 1