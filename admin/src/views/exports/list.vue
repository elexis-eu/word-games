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
      <el-table-column label="Type" width="200">
        <template slot-scope="scope">
          {{ exportTypesIndex[scope.row.type] }}
        </template>
      </el-table-column>
      <el-table-column label="Filename" width="300" align="center">
        <template slot-scope="scope">
          <span>{{ scope.row.filename }}</span>
        </template>
      </el-table-column>
      <el-table-column label="User" align="center">
        <template slot-scope="scope">
          {{ scope.row.email }}
        </template>
      </el-table-column>
      <el-table-column label="From date" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.date_from | moment("D. M. YYYY")  }}
        </template>
      </el-table-column>
      <el-table-column label="To date" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.date_to | moment("D. M. YYYY")  }}
        </template>
      </el-table-column>
      <el-table-column label="Created" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.created | moment("D. M. YYYY H:mm")  }}
        </template>
      </el-table-column>
      <el-table-column label="Started" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.started | moment("D. M. YYYY H:mm")  }}
        </template>
      </el-table-column>
      <el-table-column label="Finished" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.finished | moment("D. M. YYYY H:mm")  }}
        </template>
      </el-table-column>

      <el-table-column label="Action" width="200" align="center" v-if="this.$store.state.user.role == 'admin'">
        <template slot-scope="scope">
          <el-button type="primary" icon="el-icon-download" v-on:click="downloadFile(scope.row.filename)"></el-button>
        </template>
      </el-table-column>

    </el-table>
    <el-pagination :total="totalDocs" :current-page="page" :page-size="limit" :hide-on-single-page="true" @current-change="handlePageChange"/>
  </div>
</template>

<script>
import { getList, getFile, exportTypes } from '@/api/exports'

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
      exportTypesIndex: [],
      page: 1,
      limit: 15,
      totalDocs: 0
    }
  },
  created() {
    exportTypes().then(response => { response.data.forEach(type => this.exportTypesIndex[type.code] = type.title) })
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
    downloadFile(filename) {
      getFile(filename)
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
