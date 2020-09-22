<template>
  <div class="app-container">
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
      <el-table-column label="Type">
        <template slot-scope="scope">
          {{ importTypesIndex[scope.row.type] }}
        </template>
      </el-table-column>
      <el-table-column label="User" align="center">
        <template slot-scope="scope">
          <span>{{ scope.row.email }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Filename" align="center">
        <template slot-scope="scope">
          <span>{{ scope.row.filename }}</span>
        </template>
      </el-table-column>
      <!--<el-table-column label="Delimiter" width="110" align="center">
        <template slot-scope="scope">
          {{ scope.row.delimiter }}
        </template>
      </el-table-column>-->
      <el-table-column label="Status" width="110" align="center">
        <template slot-scope="scope">
          {{ scope.row.status }}
        </template>
      </el-table-column>
      <el-table-column label="Status" width="110" align="center">
        <template slot-scope="scope">
          <el-progress v-if="scope.row.task_all > 0" :text-inside="true" :stroke-width="26" :percentage="Math.floor((scope.row.task_done/scope.row.task_all)*100)"></el-progress>
          <span v-if="scope.row.task_all == 0">No changes!</span>
        </template>
      </el-table-column>
      <el-table-column label="Created" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.created | moment("D. M. YYYY, H:mm")  }}
        </template>
      </el-table-column>
      <!--<el-table-column label="Started" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.started | moment("D. M. YYYY, H:mm")  }}
        </template>
      </el-table-column>
      <el-table-column label="Finished" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.finished | moment("D. M. YYYY, H:mm")  }}
        </template>
      </el-table-column>-->

      <el-table-column label="Action" width="200" align="center" v-if="this.$store.state.user.role == 'admin'">
        <template slot-scope="scope">
          <router-link :to="{name: 'detailImports', params: {id: scope.row.id}}" tag="a" class="el-button el-button--primary el-button--small">
            <svg-icon icon-class="file-alt" />
            <span>Details</span>
          </router-link>
        </template>
      </el-table-column>

    </el-table>
    <el-pagination :total="totalDocs" :current-page="page" :page-size="limit" :hide-on-single-page="true" @current-change="handlePageChange"/>
  </div>
</template>

<script>
import { getList, importTypes } from '@/api/imports'

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
      importTypesIndex: [],
      importTypes: [],
      page: 1,
      limit: 15,
      totalDocs: 0
    }
  },
  created() {
    importTypes().then(response => { response.data.forEach(type => this.importTypesIndex[type.code] = type.title) })
    this.fetchData()
  },
  methods: {
    fetchData() {
      this.listLoading = true
      getList(this.page).then(response => {
        this.list = response.data
        
        this.totalDocs = response.total
        this.page = parseInt(response.page)
        this.limit = response.limit

        this.listLoading = false
      })
    },
    getImportType(code) {

    },
    handlePageChange(value) {
      const page = parseInt(value)
      this.page = (isNaN(page)) ? 1 : page
      this.$router.push({ query: { page: this.page }})
      this.fetchData()
    }
  }
}
</script>
