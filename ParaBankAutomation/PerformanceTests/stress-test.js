import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '1m', target: 20 },    // Ramp to 20 users
    { duration: '2m', target: 20 },    // Hold at 20
    { duration: '1m', target: 50 },    // Push to 50
    { duration: '2m', target: 50 },    // Hold at 50
    { duration: '1m', target: 100 },   // Push to breaking point
    { duration: '2m', target: 100 },   // Hold at peak
    { duration: '1m', target: 0 },     // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<10000'],
    http_req_failed: ['rate<0.3'],
  },
};

const BASE_URL = 'https://parabank.parasoft.com/parabank';

export default function () {
  // Full user journey
  const loginRes = http.post(
    `${BASE_URL}/login.htm`,
    'username=john&password=demo',
    { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }
  );

  check(loginRes, { 'login successful': (r) => r.status === 200 });

  sleep(1);

  http.get(`${BASE_URL}/overview.htm`);
  sleep(2);

  http.get(`${BASE_URL}/transfer.htm`);
  sleep(1);

  http.get(`${BASE_URL}/logout.htm`);
  sleep(1);
}