import request from '@/utils/request'

export function getList(page) {
  return request({
    url: '/api/v1/admin/exports/list',
    method: 'get',
    params: {page}
  })
}

export function getOne(id) {
  return request({
    url: '/api/v1/admin/exports/get',
    method: 'get',
    params: { id } 
  })
}


export function getFile(filename) {

  request({
    url: '/api/v1/admin/exports/file',
    method: 'get',
    params: { filename },
    responseType: 'blob',
  }).then((response) => {

     let fileURL = window.URL.createObjectURL(new Blob([response]));

     let fileLink = document.createElement('a');

     fileLink.href = fileURL;

     fileLink.setAttribute('download', filename);

     document.body.appendChild(fileLink);

     fileLink.click();
  });
}

export function save(id, data) {
  return request({
    url: '/api/v1/admin/exports/save',
    method: 'post',
    params: { id },
    data
  })
}

export function exportTypes() {
  return request({
    url: '/api/v1/admin/crons/list',
    method: 'get',
    params: { type: "export" }
  })

  return [
    {title: "Collocations dictionary", code: "col_dict"},
    {title: "Synonyms dictionary", code: "synonyms_dict"},
  ]
}
