<template>
  <div class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="240px" v-if="this.$store.state.user.role == 'admin'">
      <el-row>
          <el-col :span="12">
            <el-form-item label="Collocations Solo">
              <el-switch v-model="form.collocations_solo" :active-value="1" :inactive-value="0"/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Collocations Multiplayer">
              <el-switch v-model="form.collocations_multiplayer" :active-value="1" :inactive-value="0"/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
            <el-row>
          <el-col :span="12">
            <el-form-item label="Synonyms Solo">
              <el-switch v-model="form.synonyms_solo" :active-value="1" :inactive-value="0"/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
            <el-row>
          <el-col :span="12">
            <el-form-item label="Synoyms Multiplayer">
              <el-switch v-model="form.synonyms_multiplayer" :active-value="1" :inactive-value="0"/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-form-item>
        <el-button type="primary" @click="onSave">Save</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>

import { getOne, edit } from '@/api/gamemodes'
import { Message } from 'element-ui'

export default {
  data() {
    return {
      form: {
        collocations_solo: 1,
        collocations_multiplayer: 1,
        synonyms_solo: 1,
        synonyms_multiplayer: 1,
      },
      rules: {
      },
    }
  },
  created() {

      getOne()
        .then(response => {
          this.form = response.data
          this.isChanged = false
        })
        .catch(error => {
          console.log(error)
          this.isChanged = true
        }).finally(() => {
          this.submitted = false
        })
  },
  methods: {
    onSave(redirect = false) {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          const successMessage = 'Game modes updated successfully!'

          this.submitted = true

          edit(this.form)
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
              this.submitted = false
            })
        }
      })
    },
    onCancel() {
      this.$message({
        message: 'cancel!',
        type: 'warning'
      })
    }
  }
}
</script>

<style scoped>
.line{
  text-align: center;
}
</style>

