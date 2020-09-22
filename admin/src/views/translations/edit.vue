<template>
  <div class="json-editor">
    <textarea ref="textarea" />
    <el-divider/>
    <el-row>
      <el-col style="margin-left: 20px">
        <el-button type="primary" @click="onSave">Save</el-button>
      </el-col>
    </el-row>
  </div>
</template>

<script>

import { getOne, save, defaultTranslation} from '@/api/translations'
import { Message } from 'element-ui'

import CodeMirror from 'codemirror'
import 'codemirror/addon/lint/lint.css'
import 'codemirror/lib/codemirror.css'
import 'codemirror/theme/rubyblue.css'
require('script-loader!jsonlint')
import 'codemirror/mode/javascript/javascript'
import 'codemirror/addon/lint/lint'
import 'codemirror/addon/lint/json-lint'

export default {
  name: 'JsonEditor',
  /* eslint-disable vue/require-prop-types */
  props: ['value'],
  data() {
    return {
      jsonEditor: false,
      code: defaultTranslation()
    }
  },
  created() {
      getOne(this.code)
        .then(response => {
          this.jsonEditor.setValue(JSON.stringify(response.data, null, 2))
        })
        .catch(error => {
          console.log(error)
        }).finally(() => {
        })
  },
  watch: {
    value(value) {
      const editorValue = this.jsonEditor.getValue()
      if (value !== editorValue) {
        this.jsonEditor.setValue(JSON.stringify(this.value, null, 2))
      }
    }
  },
  mounted() {
    this.jsonEditor = CodeMirror.fromTextArea(this.$refs.textarea, {
      lineNumbers: true,
      mode: 'application/json',
      gutters: ['CodeMirror-lint-markers'],
      theme: 'rubyblue',
      lint: true
    })
    this.jsonEditor.setValue(JSON.stringify(this.value, null, 2))
    this.jsonEditor.on('change', cm => {
      this.$emit('changed', cm.getValue())
      this.$emit('input', cm.getValue())
    })
  },
  methods: {
    getValue() {
      return this.jsonEditor.getValue()
    },
    onSave(redirect = false) {
      const successMessage = 'Translation saved successfully!'

      save(this.code, this.jsonEditor.getValue())
        .then(response => {
          Message({
            message: successMessage,
            type: 'success',
            duration: 5 * 1000
          })

        })
        .catch(error => {
          console.log(error)
          console.log(error.message)
          console.log(error.data)
        }).finally(() => {

        })
    }
  }
}
</script>

<style scoped>
.json-editor{
  height: 100%;
  position: relative;
}
.json-editor >>> .CodeMirror {
  height: auto;
  min-height: 300px;
}
.json-editor >>> .CodeMirror-scroll{
  min-height: 300px;
}
.json-editor >>> .cm-s-rubyblue span.cm-string {
  color: #F08047;
}
</style>
