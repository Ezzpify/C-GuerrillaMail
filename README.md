# C-GuerrillaMail
C# Class for GuerrillaMail Temporary email service

Need to quickly get an email adress and an activation link? Or just get an email? Who knows what plans you have, but this will help you out if you prefer to do things in C#.

Get a temporary and disposable email to quickly use using the GuerrillaMail public API.

Responses will come in Json and the project relies on Json.NET (included in the project).
You can also check out their website http://www.newtonsoft.com/json

```
string GetAllEmails()
string GetEmailsSinceID(string mail_id)
string GetLastEmail()
string GetMyEmail(int domain = 0)
void DeleteEmails(string[] mail_ids)
void DeleteSingleEmail(string mail_id)
```

Email responses will look like the following:

```
{
    "mail_id":"53312559",
    "mail_from":"somemail@outlook.com",
    "mail_subject":"Activation email",
    "mail_excerpt":"Hey click this link to activate your account http://www.google.com thank you",
    "mail_timestamp":"1441362791",
    "mail_read":"0",
    "mail_date":"9:32:15",
    "att":"0",
    "mail_size":"1050"
}
```

Check out https://www.guerrillamail.com/ for direct service

This class is free to use for any purpose.
