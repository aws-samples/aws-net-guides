let backendHost;

const hostname = window && window.location && window.location.hostname;

if (hostname === 'amazonaws.com') {
    backendHost = '/Prod';
} else {
    backendHost = process.env.REACT_APP_BACKEND_HOST || 'http://localhost:49900';
}

export const API_ROOT = `${backendHost}/api`;