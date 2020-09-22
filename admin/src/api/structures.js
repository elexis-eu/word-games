import request from '@/utils/request'

export function getList() {
  return request({
    url: '/api/v1/admin/structures/list',
    method: 'get'
  })
}

export function getOne(id) {
  return request({
    url: '/api/v1/admin/structures/get',
    method: 'get',
    params: { id } 
  })
}

export function deleteOne(id) {
  return request({
    url: '/api/v1/admin/structures/delete',
    method: 'get',
    params: { id } 
  })
}

export function save(id, data) {
  return request({
    url: '/api/v1/admin/structures/save',
    method: 'post',
    params: { id },
    data
  })
}