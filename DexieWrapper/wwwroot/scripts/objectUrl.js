const dict = {};
const runInBrowser = typeof window !== 'undefined';

export function createObjectUrl(blob) {
    if (runInBrowser) {
        return URL.createObjectURL(blob);
    } else {
        const objectUrl = generateUUID();
        dict[objectUrl] = blob;
        return objectUrl;
    }
}

export function createObjectUrlFromUint8Array(data, mimeType) {
    if (runInBrowser) {
        const blob = new Blob([data], { type: mimeType });
        return URL.createObjectURL(blob);
    } else {
        const objectUrl = generateUUID();
        dict[objectUrl] = { data: data, type: mimeType };
        return objectUrl;
    }
}

export function revokeObjectUrl(objectUrl) {
    if (runInBrowser) {
        URL.revokeObjectURL(objectUrl);
    } else {
        delete dict[objectUrl];
    }
}

export async function fetchObjectUrl(objectUrl) {
    if (runInBrowser) {
        return await fetch(objectUrl).then(r => r.blob());
    } else {
        return dict[objectUrl];
    }
}

export async function fetchObjectUrlAsUint8Array(objectUrl) {
    if (runInBrowser) {
        let blob = await fetch(objectUrl).then(r => r.blob());
        var data = new Uint8Array(await new Response(blob).arrayBuffer());
        return data;
    } else {
        var blob = dict[objectUrl];
        if (!blob) {
            throw "Not found"
        }
        return dict[objectUrl].data;
    }
}

function generateUUID() { // Public Domain/MIT
    let d = new Date().getTime();//Timestamp
    let d2 = ((typeof performance !== 'undefined') && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16;//random number between 0 and 16
        if (d > 0) {//Use timestamp until depleted
            r = (d + r) % 16 | 0;
            d = Math.floor(d / 16);
        } else {//Use microseconds since page-load if supported
            r = (d2 + r) % 16 | 0;
            d2 = Math.floor(d2 / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

