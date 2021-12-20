/*
* Stress test for GET /liquidity-pools with filters.
* Created due to anomalous 10-20s response times seen on other tests.
 */

import http from 'k6/http';

export let options = {
    stages: [
        { duration: '1m', target: 5 },
        { duration: '3m', target: 5 },
        { duration: '1m', target: 10 },
        { duration: '3m', target: 10 },
        { duration: '1m', target: 50 },
        { duration: '3m', target: 50 },
        { duration: '1m', target: 100 },
        { duration: '3m', target: 100 },
        { duration: '5m', target: 0 }
    ]
};

const API_BASE_URL = __ENV.api_base_url;
const MARKET = 't8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH';

export default () => {
    var authRequest = http.post(`${API_BASE_URL}/auth/authorize?wallet=tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm`);

    const params = { headers: { 'Authorization': `Bearer ${authRequest.body}` } };
    http.get(`${API_BASE_URL}/liquidity-pools?markets=${MARKET}&miningFilter=Enabled&orderBy=Liquidity&limit=4&direction=DESC`, params)
}
