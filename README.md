Test the API without web app:
1. Get access token using url: https://login.microsoftonline.com/zhenzhaad.onmicrosoft.com/oauth2/v2.0/authorize?client_id=ea9d6b1a-2db5-46f8-990c-bb133e9e570c&nonce=defaultNonce&redirect_uri=https%3a%2f%2fjwt.ms&scope=https%3a%2f%2flocalhost%3a44367%2fuser_impersonation&response_type=token&prompt=login
2. Call Web API https://localhost:44367/api/product in Postman with the Bearer token fetched in step 1
