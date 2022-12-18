const dict = {};

export function createObjectUrl(blob) {
    if (IsRunInBroser()) {
        return URL.createObjectURL(blob);
    }

    const objectUrl = generateUUID();
    dict[objectUrl] = blob;
    return objectUrl;
}

export function createObjectUrlFromUint8Array(data, mimeType) {
    if (IsRunInBroser()) {
        const blob = new Blob([data], { type: mimeType });
        return URL.createObjectURL(blob);
    }

    const objectUrl = generateUUID();
    dict[objectUrl] = data;
    return objectUrl;
}

export function revokeObjectUrl(objectUrl) {
    if (IsRunInBroser()) {
        URL.revokeObjectURL(objectUrl);
    } else {
        delete dict[objectUrl];
    }
}

export async function fetchObjectUrl(objectUrl) {
    if (IsRunInBroser()) {
        return await fetch(objectUrl).then(r => r.blob());
    }

    return dict[objectUrl];
}

export async function fetchObjectUrlAsUint8Array(objectUrl) {
    if (IsRunInBroser()) {
        let blob = await fetch(objectUrl).then(r => r.blob());
        var data = new Uint8Array(await new Response(blob).arrayBuffer());
        return data;
    }

    return dict[objectUrl];
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

function IsRunInBroser() {
    return typeof window !== 'undefined';
}
