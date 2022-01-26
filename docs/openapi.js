window.addEventListener('load', () => setTimeout(setSwaggerUI, 100));

function setSwaggerUI() {
    const domainOrigin = window.location.origin;

    // this is a full override of Swagger UI
    // Docs: https://swagger.io/docs/open-source-tools/swagger-ui/usage/configuration/
    window.ui = new SwaggerUIBundle({
        urls: [{ url: `${domainOrigin}/swagger/v1/openapi.yml`, name: 'Opdex Platform API V1' }],
        dom_id: '#swagger-ui',
        deepLinking: true,
        requestInterceptor: function (request) {
            // request interceptor
            // add custom headers here
            return request;
        },
        onComplete: function () {
            // on complete callback
        },
        presets: [
            SwaggerUIBundle.presets.apis,
            SwaggerUIStandalonePreset
        ],
        plugins: [
            SwaggerUIBundle.plugins.DownloadUrl,

            // Custom plugin that replaces the server list with the current url
            function () {
                return {
                    statePlugins: {
                        spec: {
                            wrapActions: {
                                updateJsonSpec: function (oriAction, system) {
                                    return (spec) => {
                                        spec.servers = [{url: `${domainOrigin}/v1`}]
                                        return oriAction(spec)
                                    }
                                }
                            }
                        }
                    }
                }
            }
        ],
        layout: "StandaloneLayout",
    });
}
