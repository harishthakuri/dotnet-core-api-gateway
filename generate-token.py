#!/usr/bin/env python3
"""
Simple JWT token generator for testing the API Gateway
Uses the same settings as appsettings.json
"""

import jwt
import datetime
import sys

# Configuration from appsettings.json
SECRET_KEY = "your-secret-key-min-32-characters-long-for-production"
ISSUER = "ApiGateway"
AUDIENCE = "ApiGatewayClients"

def generate_token(username="testuser", roles=None):
    """Generate a JWT token"""
    
    if roles is None:
        roles = ["Admin"]
    
    # Set expiration to 1 hour from now
    expiration = datetime.datetime.utcnow() + datetime.timedelta(hours=1)
    
    # Create claims
    claims = {
        "sub": username,
        "name": username,
        "role": roles,
        "jti": "test-" + datetime.datetime.utcnow().isoformat(),
        "iss": ISSUER,
        "aud": AUDIENCE,
        "exp": expiration,
        "iat": datetime.datetime.utcnow()
    }
    
    # Generate token
    token = jwt.encode(claims, SECRET_KEY, algorithm="HS256")
    
    return token

if __name__ == "__main__":
    token = generate_token()
    print(token)
