import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 5 },    // Normal load
    { duration: '10s', target: 50 },   // SPIKE to 50 users
    { duration: '20s', target: 50 },   // Sustain spike
    { duration: '10s', target: 5 },    // Return to normal
    { duration: '10s', target: 0 },    // Recovery
  ],
  thresholds: {
    http_req_duration: ['p(95)<5000'], // More lenient during spike
    http_req_failed: ['rate<0.2'],      // Allow 20% failure during spike
  },
};

const BASE_URL = 'https://parabank.parasoft.com/parabank';

export default function () {
  const res = http.get(`${BASE_URL}/index.htm`);
  
  check(res, {
    'status is 200': (r) => r.status === 200,
    'homepage loaded': (r) => r.body.includes('ParaBank'),
  });

  sleep(0.5);
}