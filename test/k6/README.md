# Load Tests

These tests can be run using [K6](https://k6.io). Run tests using `k6 run test-name.js -e api_base_url=https://test-api.opdex.com`, substituting the base URL for the target API host.

## Guidelines

* Make sure the IP address of the machine running is whitelisted in the API config. If not running behind a gateway, add it to `IpRateLimiting:IpWhitelist` config array.
* Always use `__ENV.api_base_url` to retrieve the API base URL within test files.
* Dates used in tests should generally not be hard coded and _always_ UTC.
* Add a multi-line comment to the top of the test file, describing the purpose of the test.
* Tests which are intended to be run as part of CI should be placed within `ci` sub-directory.
