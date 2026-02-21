import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');

// Test configuration
export const options = {
  stages: [
    { duration: '30s', target: 10 },  // Ramp up to 10 users over 30s
    { duration: '1m', target: 10 },   // Stay at 10 users for 1 minute
    { duration: '30s', target: 0 },   // Ramp down to 0 users
  ],
  thresholds: {
    http_req_duration: ['p(95)<2000'], // 95% of requests must complete below 2s
    errors: ['rate<0.1'],               // Error rate must be below 10%
  },
};

const BASE_URL = 'https://parabank.parasoft.com/parabank';

export default function () {
  // Test login functionality
  const loginPayload = {
    username: 'john',
    password: 'demo',
  };

  const params = {
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
    },
  };

  // Perform login
  const loginRes = http.post(
    `${BASE_URL}/login.htm`,
    `username=${loginPayload.username}&password=${loginPayload.password}`,
    params
  );

  // Verify login was successful
  const loginSuccess = check(loginRes, {
    'login status is 200': (r) => r.status === 200,
    'login response time < 2000ms': (r) => r.timings.duration < 2000,
    'accounts overview loaded': (r) => r.body.includes('Accounts Overview'),
  });

  errorRate.add(!loginSuccess);

  sleep(1); // Think time between requests
}