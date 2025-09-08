# Asp.Net Core OAuth 2.0 Authorization Code with PKCE Demo

This solution uses Keycloak as an authentication server. Make sure you:

1. Start dependencies using docker-compose.yml (see the `.env` file for credentials)
2. Navigate to keycloak admin ui (http://localhost:8080)
3. Create a new keycloak realm: `keycloak-test`
4. Create a new public client inside the new realm and configure it:
- Client type: OpenID Connect
- Client ID: `pkce-client`
- Client authentication: Off (this makes it a public client)
- Authorization : Off

- Authentication flow: Enable Standard flow
- Direct access grants: Off
- Implicit flow: Off
- OAuth 2.0 Device Authorization Grant: Off

- PKCE Method: S256

- Valid redirect URIs: `http://localhost:5276/swagger/oauth2-redirect.html` (should be exact url)
- Web origins: `http://localhost:5276`

5. If needed (if you used different realm or client name) modify `appsettings.Development.json`
6. Start the solution and authenticate through Swagger UI:
  - client_id: `pkce_client`
  - client_secret: leave empty
