import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 5,           // 5 virtual users
  duration: '30s',  // Run for 30 seconds
};

export default function () {
  const res = http.get('https://parabank.parasoft.com/parabank/index.htm');
  
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 2s': (r) => r.timings.duration < 2000,
  });
  
  sleep(1);
}