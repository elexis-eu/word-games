import request from '@/utils/request'

export function edit(data) {
  return request({
    url: '/api/v1/admin/gamemodes/save',
    method: 'post',
    data
  })
}

export function getOne() {
  return request({
    url: '/api/v1/admin/gamemodes/get',
    method: 'get'
  })
}