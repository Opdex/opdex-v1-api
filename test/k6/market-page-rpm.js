/*
* Tests that the API can handle 250 separate users accessing the UI market page per minute, for a period of 5 minutes.
* This assumes no data on the UI is loaded from cache.
 */

import http from 'k6/http';

export let options = {
    scenarios: {
        constant_request_rate: {
            executor: 'constant-arrival-rate',
            rate: 250,
            timeUnit: '1m',
            duration: '5m',
            preAllocatedVUs: 20,
            maxVUs: 100
        }
    }
};

const API_BASE_URL = __ENV.api_base_url;
const MARKET = 't8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH';
const today = new Date();
const lastWeekYesterday = new Date(today.valueOf());
lastWeekYesterday.setUTCDate(today.getUTCDate() - 8);
const lastYearToday = new Date(today.valueOf())
lastYearToday.setUTCFullYear(today.getUTCFullYear() - 1);

let yyyymmdd = date => date.toISOString().slice(0, 10);

export default () => {
    var authRequest = http.post(`${API_BASE_URL}/auth/authorize?wallet=tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm`);

    const params = { headers: { 'Authorization': `Bearer ${authRequest.body}` } };
    http.batch([
        { method: 'GET', params: params, url: `${API_BASE_URL}/index/latest-block` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/markets/${MARKET}` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/markets/${MARKET}/history?startDateTime=${yyyymmdd(lastYearToday)}T00:00:00.000Z&endDateTime=${yyyymmdd(today)}T23:59:59.999Z&interval=OneDay&limit=750&direction=ASC` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/liquidity-pools?markets=${MARKET}&orderBy=Liquidity&limit=5&direction=DESC` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/markets/${MARKET}/tokens?tokenType=NonProvisional&orderBy=DailyPriceChangePercent&limit=5&direction=DESC` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/markets/${MARKET}/tokens/tFPedNjm3q8N9HD7wSVTNK5Kvw96332P1o/history?startDateTime=${yyyymmdd(lastWeekYesterday)}T00:00:00.000Z&endDateTime=${yyyymmdd(today)}T23:59:59.999Z&interval=OneDay&limit=750&direction=ASC` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/markets/${MARKET}/tokens/tPXUEzDyZDrR8YzQ6LiAJWhVuAKB8RUjyt/history?startDateTime=${yyyymmdd(lastWeekYesterday)}T00:00:00.000Z&endDateTime=${yyyymmdd(today)}T23:59:59.999Z&interval=OneDay&limit=750&direction=ASC` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/markets/${MARKET}/tokens/tHb7w3dpzq9d8uBVYRrYrqHfBdoMZXqTzG/history?startDateTime=${yyyymmdd(lastWeekYesterday)}T00:00:00.000Z&endDateTime=${yyyymmdd(today)}T23:59:59.999Z&interval=OneDay&limit=750&direction=ASC` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/markets/${MARKET}/tokens/tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3/history?startDateTime=${yyyymmdd(lastWeekYesterday)}T00:00:00.000Z&endDateTime=${yyyymmdd(today)}T23:59:59.999Z&interval=OneDay&limit=750&direction=ASC` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/markets/${MARKET}/tokens/tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD/history?startDateTime=${yyyymmdd(lastWeekYesterday)}T00:00:00.000Z&endDateTime=${yyyymmdd(today)}T23:59:59.999Z&interval=OneDay&limit=750&direction=ASC` },
        { method: 'GET', params: params, url: `${API_BASE_URL}/liquidity-pools?markets=${MARKET}&miningFilter=Enabled&orderBy=Liquidity&limit=4&direction=DESC` }
    ]);
}
