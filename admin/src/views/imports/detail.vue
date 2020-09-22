<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>Import Info</span>
      </div>
      <div>
      <el-row class="row-data">
          <el-col :span="12">
            Type
          </el-col>
          <el-col :span="12">
            {{ importData.type }}
          </el-col>
      </el-row>
      <el-row class="row-data">
          <el-col :span="12">
            Filename
          </el-col>
          <el-col :span="12">
            {{ importData.filename }}
          </el-col>
      </el-row>
      <el-row class="row-data">
          <el-col :span="12">
            Delimiter
          </el-col>
          <el-col :span="12">
            {{ importData.delimiter }}
          </el-col>
      </el-row>
      <el-row class="row-data">
          <el-col :span="12">
            Status
          </el-col>
          <el-col :span="12">
            {{ importData.status }}
          </el-col>
      </el-row>
      <el-row class="row-data">
          <el-col :span="12">
            Progress            
          </el-col>
          <el-col :span="12">
            <el-progress v-if="importData.task_all > 0" :text-inside="true" :stroke-width="26" :percentage="(importData.task_done/importData.task_all)*100"></el-progress>
            <span v-if="importData.task_all == 0">No changes!</span>
          </el-col>
      </el-row>
      <el-row class="row-data">
          <el-col :span="12">
            Created
          </el-col>
          <el-col :span="12">
            {{ importData.created | moment("D. M. YYYY, H:mm")  }}
          </el-col>
      </el-row>
      <el-row class="row-data">
          <el-col :span="12">
            Started
          </el-col>
          <el-col :span="12">
            {{ importData.started | moment("D. M. YYYY, H:mm")  }}
          </el-col>
      </el-row>
      <el-row class="row-data">
          <el-col :span="12">
            Finished
          </el-col>
          <el-col :span="12">
            {{ importData.finished | moment("D. M. YYYY, H:mm")  }}
          </el-col>
      </el-row>
      </div>
    </el-card>
    <el-divider/>
    <el-card>
      <div slot="header" class="clearfix">
        <span>Error log</span>
      </div>
      <div>
        <el-table
        v-loading="listLoading"
        :data="list"
        element-loading-text="Loading"
        border
        fit
        highlight-current-row
      >
        <el-table-column align="center" label="ID" width="95">
          <template slot-scope="scope">
            {{ scope.row.id }}
          </template>
        </el-table-column>
        <el-table-column label="Line">
          <template slot-scope="scope">
            <pre>{{ scope.row.line }}</pre>
          </template>
        </el-table-column>
        <el-table-column label="Error" align="center">
          <template slot-scope="scope">
            <pre>{{ scope.row.error }}</pre>
          </template>
        </el-table-column>
        <el-table-column label="Created" width="200" align="center">
          <template slot-scope="scope">
            {{ scope.row.created | moment("D. D. YYYY, H:mm")  }}
          </template>
        </el-table-column>
      </el-table>
      </div>
    </el-card>
  </div>
</template>

<script>

import { getOne, getLogs } from '@/api/imports'

export default {
  filters: {
    statusFilter(status) {
      const statusMap = {
        published: 'success',
        draft: 'gray',
        deleted: 'danger'
      }
      return statusMap[status]
    }
  },
  data() {
    return {
      list: null,
      listLoading: true,
      importData: {},
      refreshInterval: null
    }
  },
  created() {
    this.fetchData()
  },
  destroyed() {
    clearInterval(this.refreshInterval)
  },
  methods: {
    fetchData() {

    if (this.$route.params.id) {
      this.id = this.$route.params.id

      getOne(this.id)
        .then(response => {
          this.importData = response.data
          
          if(this.importData.status != 'finish'){
            this.startRefreshing()
          }
        })
        .catch(error => {
          console.log(error)
        }).finally(() => {
        })
    } else {

    }

      this.listLoading = true
      getLogs(this.id).then(response => {
        this.list = response.data
        this.listLoading = false
      })
    },
    startRefreshing: function () {
                   this.refreshInterval = setInterval(() => {
                      getOne(this.id)
                        .then(response => {
                          this.importData = response.data
                        })
                        .catch(error => {
                          console.log(error)
                        }).finally(() => {

                        })
                   }, 1000);
          }
  }
}
</script>

<style>
  .text {
    font-size: 14px;
  }

  .item {
    margin-bottom: 18px;
  }

  .clearfix:before,
  .clearfix:after {
    display: table;
    content: "";
  }
  .clearfix:after {
    clear: both
  }

  .box-card {
    width: 480px;
  }

  .row-data {
    padding-bottom: 5px;
    margin-bottom: 15px;
    border-bottom: 1px solid #EBEEF5;
  }
</style>
