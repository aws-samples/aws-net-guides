// function CallRemote(remoteHost, path, httpMethod) {
//     const remoteResponse = document.getElementById('remoteResponse');
//     //const myHeaders = includeHeaders ? { 'Content-Type': 'x-custom-header' } : {};

//     fetch(`${remoteHost}${path}`,
//     {
//         method: httpMethod,
//     }).then(response => {
//         if (response.ok) {
//             response.text().then(text => {
//                 remoteResponse.innerText = text;
//             });
//         }
//         else {
//             remoteResponse.innerText = response.status;
//         }
//     })
//     .catch(() => remoteResponse.innerText = 'An error occurred, might be CORS?! :) Press F12 to open the web debug tools');
// }
