# Vulnerable API Demo

This project demonstrates a vulnerable ASP.NET Core API application, specifically showcasing a mass assignment vulnerability. It's designed for educational purposes to help developers understand and identify security risks in web applications.

## Getting Started

### Prerequisites

- Docker

### Running the Application

1. Clone this repository
2. Navigate to the project directory
3. Build the Docker image:
   ```
   docker build -t vulnerable-api .
   ```
4. Run the container:
   ```
   docker run -p 8880:8880 vulnerable-api
   ```

The API will now be accessible at `http://localhost:8880`.

## API Endpoints

### 1. Vulnerable Signup (Mass Assignment Vulnerability)

**Endpoint:** `POST /api/auth/signup`

This endpoint demonstrates a mass assignment vulnerability where a user can set their admin status during signup.

**Example (Non-admin user):**

```bash
curl -X POST http://localhost:8880/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","name":"Regular User","password":"password123","isAdmin":false}'
```

**Example (Admin user - exploiting vulnerability):**

```bash
curl -X POST http://localhost:8880/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","name":"Admin User","password":"password123","isAdmin":true}'
```

### 2. Secure Signup (Protected against Mass Assignment)

**Endpoint:** `POST /api/auth/signup-secure`

This endpoint demonstrates a secure signup process that prevents users from setting their admin status.

**Example:**

```bash
curl -X POST http://localhost:8880/api/auth/signup-secure \
  -H "Content-Type: application/json" \
  -d '{"email":"secure@example.com","name":"Secure User","password":"password123"}'
```

Note: Attempting to include `"isAdmin":true` in this request will have no effect, as the property is ignored.

### 3. Login

**Endpoint:** `POST /api/auth/login`

This endpoint allows users to log in and receive a JWT token.

**Example:**

```bash
curl -X POST http://localhost:8880/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'
```

## Demonstrating the Vulnerability

1. Sign up a regular user using the vulnerable endpoint:

```bash
curl -X POST http://localhost:8880/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","name":"Regular User","password":"password123","isAdmin":false}'
```

2. Sign up an admin user exploiting the mass assignment vulnerability:

```bash
curl -X POST http://localhost:8880/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","name":"Admin User","password":"password123","isAdmin":true}'
```

3. Log in with the admin user:

```bash
curl -X POST http://localhost:8880/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password123"}'
```

You'll receive a JWT token. If you decode this token (e.g., using jwt.io), you'll see that the "role" claim is set to "Admin", demonstrating that the user was able to create an admin account due to the mass assignment vulnerability.

## Demonstrating the Secure Version

1. Try to sign up an admin user using the secure endpoint:

```bash
curl -X POST http://localhost:8880/api/auth/signup-secure \
  -H "Content-Type: application/json" \
  -d '{"email":"secure@example.com","name":"Secure User","password":"password123","isAdmin":true}'
```

2. Log in with this user:

```bash
curl -X POST http://localhost:8880/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"secure@example.com","password":"password123"}'
```

If you decode the JWT token received from this login, you'll see that the "role" claim is set to "User", demonstrating that the secure signup process ignored the `isAdmin` field and created a regular user account.

## Security Considerations

This application intentionally contains vulnerabilities for educational purposes. Do not use this code in a production environment. Always validate and sanitize user input, and be cautious about mass assignment vulnerabilities in your applications.

## License

This project is licensed under the MIT License - see the LICENSE file for details.