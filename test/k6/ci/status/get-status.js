/*
* Performance test for GET /status
 */

import http from 'k6/http';

export let options = {
    scenarios: {
        constant_request_rate: {
            executor: 'constant-arrival-rate',
            rate: 1,
            timeUnit: '1s',
            duration: '30s',
            preAllocatedVUs: 1,
            maxVUs: 10
        }
    },
    thresholds: {
        http_req_failed: [ 'rate<0.01' ],
        http_req_duration: [ 'p(99)<250' ]
    }
};

const API_BASE_URL = __ENV.api_base_url;

export default () => {
    const response = http.get(`${API_BASE_URL}/status`);
}
