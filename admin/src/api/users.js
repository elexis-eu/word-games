import request from '@/utils/request'

export function login(data) {
  return request({
    url: '/api/v1/admin/users/login',
    method: 'post',
    data
  })
}

export function getInfo(token) {
  return request({
    url: '/api/v1/admin/users/info',
    method: 'get'
  })
}

export function logout() {
  return request({
    url: '/api/v1/admin/users/logout',
    method: 'post'
  })
}

export function add(data) {
  return request({
    url: '/api/v1/admin/users/add',
    method: 'post',
    data
  })
}

export function edit(id, data) {
  return request({
    url: '/api/v1/admin/users/edit',
    method: 'post',
    params: { id },
    data
  })
}

export function getList() {
  return request({
    url: '/api/v1/admin/users/list',
    method: 'get'
  })
}

export function getOne(id) {
  return request({
    url: '/api/v1/admin/users/get',
    method: 'get',
    params: { id } 
  })
}