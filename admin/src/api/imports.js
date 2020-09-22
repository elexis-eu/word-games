import request from '@/utils/request'

export function getList(page) {
  return request({
    url: '/api/v1/admin/imports/list',
    method: 'get',
    params: { page }
  })
}

export function getLogs(id) {
  return request({
    url: '/api/v1/admin/imports/logs',
    method: 'get',
    params: { id }
  })
}

export function getOne(id) {
  return request({
    url: '/api/v1/admin/imports/get',
    method: 'get',
    params: { id } 
  })
}

export function save(id, data) {
  return request({
    url: '/api/v1/admin/imports/save',
    method: 'post',
    params: { id },
    data
  })
}

export function importTypes() {

  return request({
    url: '/api/v1/admin/crons/list',
    method: 'get',
    params: { type: "import" }
  })

  return [
    {title: "Collocations dictionary", code: "col_dict"},
    {title: "Collocations levels headwords", code: "col_levels_headword"},
    {title: "Collocations levels title", code: "col_levels_title"},
    {title: "Synonyms dictionary", code: "synonyms_dict"},
    {title: "Synonyms levels", code: "synonyms_levels"},
  ]
}
