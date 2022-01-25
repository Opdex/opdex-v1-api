// (function() {
//     const checkExists = setInterval(function() {
//         const servers = document.getElementsByClassName('servers');
//
//         if (!!servers && servers.length > 0) {
//             const server = servers[0];
//             replaceServerRoutes(server);
//             clearInterval(checkExists);
//         }
//     }, 100);
// })();
//
// // Find and replace server routes from default to the hosted domain of the current swagger instance
// function replaceServerRoutes(server) {
//     const domain = new URL(document.location.href);
//     const domainReplacement = `${domain.origin}/v1`;
//
//     // Loop through each server
//     for(let i = 0; i < server.childNodes.length; i++) {
//         const serverLabel = server.childNodes[i];
//
//         // Loop through each server label
//         if (serverLabel.childNodes.length) {
//             for(let j = 0; j < serverLabel.childNodes.length; j++) {
//                 const serverLabelSelect = serverLabel.childNodes[j];
//                 serverLabelSelect.value = domainReplacement;
//
//                 // Loop through each server label's select dropdown (should only have 1)
//                 for(let t = 0; t < serverLabelSelect.childNodes.length; t++) {
//                     const severLabelSelectOption = serverLabelSelect.childNodes[j];
//
//                     // Loop through each select option and change to the current host
//                     if (!severLabelSelectOption.value.includes(domainReplacement)) {
//                         severLabelSelectOption.value = domainReplacement;
//                         severLabelSelectOption.text = domainReplacement;
//                     }
//                 }
//             }
//         }
//     }
// }
