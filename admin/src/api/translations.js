import request from '@/utils/request'

export function getOne(code) {
  return request({
    url: '/api/v1/admin/translations/get',
    method: 'get',
    params: { code } 
  })
}

export function save(code, data) {
  return request({
    url: '/api/v1/admin/translations/save',
    method: 'post',
    data: {"json_file": data},
    params: { code } 
  })
}

export function defaultTranslation() {
  if(process.env.DEFAULT_TRANSLATION){
    return process.env.DEFAULT_TRANSLATION;
  } else {
    return 'sl';
  }
}
