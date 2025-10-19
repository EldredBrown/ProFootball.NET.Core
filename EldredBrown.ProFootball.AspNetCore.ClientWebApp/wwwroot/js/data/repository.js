const getData = async (resource) => {
    const url = `${api}/${resource}`;

    let data = null;

    await fetch(url, {
        method: "GET"
    }).then(async response => {
        data = await response.json();
    }).catch(async error => {
        console.log(await error);
    });

    return data;
};

const postData = async (resource, data) => {
    await postOrPutData(resource, "POST", data);
};

const putData = async (resource, data) => {
    await postOrPutData(resource, "PUT", data);
};

const deleteData = async (resource) => {
    const url = `${api}/${resource}`;

    await fetch(url, {
        method: "DELETE"
    }).then(async response => {
        console.log(await response);
    }).catch(async error => {
        console.log(await error);
    });
};

const postOrPutData = async (resource, httpVerb, data) => {
    const url = `${api}/${resource}`;

    await fetch(url, {
        method: httpVerb,
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    }).then(async response => {
        console.log(await response);
    }).catch(async error => {
        console.log(await error);
    });
};

export { getData, postData, putData, deleteData };
